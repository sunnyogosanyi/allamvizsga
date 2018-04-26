#include <pololu/3pi.h>
#include <stdio.h>
#include <string.h>
#include <time.h>

unsigned int last_proportional = 0;
unsigned int sensors[5];
char message[20];

const char welcome[] PROGMEM = ">g32>>c32";
const char go[] PROGMEM = "L16 cdegreg4";
const char stopSound[] PROGMEM = "L10 abcd4";
int progress =0;

int leng=0;
char received[10] = "STOP";

int motorSpeed = 30;
int leftMessageSent = 0;
int rightMessageSent = 0;
int forwardMessageSent = 0;
char* direction = "*R";
int firstPoint = 1;

long startTime, endTime;



// A global ring buffer for data coming in.  This is used by the
// read_next_byte() and previous_byte() functions, below.
char buffer[100];

// A pointer to where we are reading from.
unsigned char read_index = 0;


const char levels[] PROGMEM = {
	0b00000,
	0b00000,
	0b00000,
	0b00000,
	0b00000,
	0b00000,
	0b00000,
	0b11111,
	0b11111,
	0b11111,
	0b11111,
	0b11111,
	0b11111,
	0b11111
};

// go into program space.
const char welcome_line1[] PROGMEM = " Pololu";
const char welcome_line2[] PROGMEM = "3\xf7 Robot";
const char demo_name_line1[] PROGMEM = "Line";
const char demo_name_line2[] PROGMEM = "follower";


// Waits for the next byte and returns it.  Runs play_check to keep
// the music playing and calls pid_check() to keep following the line.
char read_next_byte()
{
    char ret = '^';
    if (serial_get_received_bytes() != read_index)
    {
        ret = buffer[read_index];
        read_index ++;
        if(read_index >= 100)
        read_index = 0;
    }
    
    return ret;
}


void load_custom_characters()
{
    lcd_load_custom_character(levels+0,0); // no offset, e.g. one bar
    lcd_load_custom_character(levels+1,1); // two bars
    lcd_load_custom_character(levels+2,2); // etc...
    lcd_load_custom_character(levels+3,3);
    lcd_load_custom_character(levels+4,4);
    lcd_load_custom_character(levels+5,5);
    lcd_load_custom_character(levels+6,6);
    clear(); // the LCD must be cleared for the characters to take effect
}

// This function displays the sensor readings using a bar graph.
void display_readings(const unsigned int *calibrated_values)
{
    unsigned char i;

    for(i=0;i<5;i++) {
        // Initialize the array of characters that we will use for the
        // graph.  Using the space, an extra copy of the one-bar
        // character, and character 255 (a full black box), we get 10
        // characters in the array.
        const char display_characters[10] = {' ',0,0,1,2,3,4,5,6,255};

        // The variable c will have values from 0 to 9, since
        // calibrated values are in the range of 0 to 1000, and
        // 1000/101 is 9 with integer math.
        char c = display_characters[calibrated_values[i]/101];

        // Display the bar graph character.
        print_character(c);
    }
}


/////////////////////////////////////////////////////////////////////
int readFromSerial()
{
    char ch;
    int index=0;
	message[0]='\0';
	
	while( ((ch = read_next_byte()) != '\n') && (ch != '^') )
    {
        message[index] = ch;
        index++;
        
    }
    message[index-1]='\0';
 
	return index;
    
}


void sendMessage(char* messag){
    int len;
    len = strlen(messag);
    
    serial_send(messag,len);
    delay_ms(100);
}

char* concat(const char *s1, const char *s2)
{
	char *result = malloc(strlen(s1)+strlen(s2)+1);//+1 for the null-terminator
	//in real code you would check for errors in malloc here
	strcpy(result, s1);
	strcat(result, s2);
	return result;
}
 


void startRobot(){
	startTime = get_ms();
	 while(progress == 1)
	 {
		 unsigned int position = read_line(sensors,IR_EMITTERS_ON);
		 lcd_goto_xy(0,1);
		 print_long(position);
		 lcd_goto_xy(0,0);
		 display_readings(sensors);
		 leng = readFromSerial();
		 
		 
		 if (leng>0)
		 {
			 //strcpy(received,message);
			 //char temp[4];
			 //itoa(position,temp,10);
			 if (strcmp(message,"stop") == 0){
			 lcd_goto_xy(3,1);
			 print(received);
			 sendMessage("INTERRUPT");
			 progress = 0;

			 }
		 }
		 
		 
		 if(position < 1500)
		 {
			 // We are far to the right of the line: turn left.
			 set_motors(0,motorSpeed);
			 left_led(1);
			 right_led(0);
			 if ((position < 1000) && (leftMessageSent == 0)){
				 endTime = get_ms();
				 long timeDiff = endTime- startTime;
				 startTime = endTime;
				 char timeBuff[50];
				 ltoa(timeDiff,timeBuff,10);
				 if ( firstPoint == 0)
				 {
					 sendMessage(concat(timeBuff,direction));
					 }else{
					 firstPoint =0;
				 }
				 direction = "*L";
				 leftMessageSent = 1;
				 rightMessageSent = 0;
				 forwardMessageSent = 0;
			 }
		 }
		 else if(position < 2500)
		 {
			 set_motors(motorSpeed,motorSpeed);
			 left_led(1);
			 right_led(1);
			 if (forwardMessageSent == 0){
				 sendMessage("F");
				 leftMessageSent = 0;
				 rightMessageSent = 0;
				 forwardMessageSent = 1;
			 }
		 }
		 else
		 {
			 // We are far to the left of the line: turn right.
			 set_motors(motorSpeed,0);
			 left_led(0);
			 right_led(1);
			 if ((position > 3000) &&(rightMessageSent == 0)){
				 endTime = get_ms();
				 long timeDiff = endTime- startTime;
				 startTime = endTime;
				 char timeBuff[50];
				 ltoa(timeDiff,timeBuff,10);
				 if ( firstPoint == 0)
				 {
					 sendMessage(concat(timeBuff,direction));
					 }else{
					 firstPoint =0;
				 }
				 direction = "*R";
				 leftMessageSent = 0;
				 rightMessageSent = 1;
				 forwardMessageSent = 0;
			 }
		 }
		 
		 
	 }
}




void initializeCommand(){
	unsigned int counter; // used as a simple timer
	unsigned int sensors[5]; // an array to hold sensor values

	// This must be called at the beginning of 3pi code, to set up the
	// sensors.  We use a value of 2000 for the timeout, which
	// corresponds to 2000*0.4 us = 0.8 ms on our 20 MHz processor.
	pololu_3pi_init(2000);
	load_custom_characters(); // load the custom characters
	

	play_from_program_space(welcome);
	while(is_playing());

	// Auto-calibration: turn right and left while calibrating the
	// sensors.
	for(counter=0;counter<80;counter++)
	{
		if(counter < 20 || counter >= 60)
		set_motors(40,-40);
		else
		set_motors(-40,40);


		calibrate_line_sensors(IR_EMITTERS_ON);

		// Since our counter runs to 80, the total delay will be
		// 80*20 = 1600 ms.
		delay_ms(20);
	}
	set_motors(0,0);

}

void startCommand(){
	print("Go!");

	// Play music and wait for it to finish before we start driving.
	play_from_program_space(go);
	while(is_playing());
	progress = 1;
	startRobot();
	
}

void stopCommand(){
	set_motors(0,0);
	play_from_program_space(stopSound);
	while(is_playing());
	
}

void batteryCommand(){
	int bat = read_battery_millivolts();

	clear();
	print_long(bat);
	print("mV");
	char messBuff[50];
	itoa(bat,messBuff,10);
	sendMessage(messBuff);
}

int main()
{
	serial_set_baud_rate(115200);
	serial_receive_ring(buffer, 100);
	
	
	while (1)
	{
	
	leng = readFromSerial();
	if (leng>0)
	{
		//strcpy(received,message);
		//char temp[4];
		//itoa(position,temp,10);
		lcd_goto_xy(3,1);
		print(message);
		if (strcmp(message,"init") == 0){
			sendMessage("initt");
			initializeCommand();
		}
		if (strcmp(message,"start") == 0){
			sendMessage("startt");
			startCommand();
		}
		if (strcmp(message,"stop") == 0){
			sendMessage("stopp");
			stopCommand();
		}
		if (strcmp(message,"battery") == 0){
			sendMessage("batteryy");
			batteryCommand();
		}
		sendMessage(message);
		//leng = 0;
	}
	
	delay_ms(100);
	}
	
}

