/*
 * 3pi-serial-slave - An example serial slave program for the Pololu
 * 3pi Robot.  See the following pages for more information:
 *
 * http://www.pololu.com/docs/0J21
 * http://www.pololu.com/docs/0J20
 * http://www.poolu.com/
 * http://forum.pololu.com
 * 
 */
#include <pololu/3pi.h>
#include <stdio.h>
#include <string.h>
#include <time.h>

// PID constants
unsigned int pid_enabled = 0;
unsigned char max_speed = 255;
unsigned char p_num = 0;
unsigned char p_den = 0;
unsigned char d_num = 0;
unsigned char d_den = 0;
unsigned int last_proportional = 0;
unsigned int sensors[5];
char message[20];
char* messType;
char* messCommand;
const char welcome[] PROGMEM = ">g32>>c32";
const char go[] PROGMEM = "L16 cdegreg4";
const char stopSound[] PROGMEM = "L10 abcd4";
int progress =0;

// This routine will be called repeatedly to keep the PID algorithm running
void pid_check()
{
    if(!pid_enabled)
        return;
    
    // Do nothing if the denominator of any constant is zero.
    if(p_den == 0 || d_den == 0)
    {
        set_motors(0,0);
        return;
    }   

    // Read the line position.
    unsigned int position = read_line(sensors, IR_EMITTERS_ON);

    // The "proportional" term should be 0 when we are on the line.
    int proportional = ((int)position) - 2000;

    // Compute the derivative (change) of the position.
    int derivative = proportional - last_proportional;

    // Remember the last position.
    last_proportional = proportional;

    // Compute the difference between the two motor power settings,
    // m1 - m2.  If this is a positive number the robot will turn
    // to the right.  If it is a negative number, the robot will
    // turn to the left, and the magnitude of the number determines
    // the sharpness of the turn.
    int power_difference = proportional*p_num/p_den + derivative*d_num/d_den;

    // Compute the actual motor settings.  We never set either motor
    // to a negative value.
    if(power_difference > max_speed)
        power_difference = max_speed;
    if(power_difference < -max_speed)
        power_difference = -max_speed;

    if(power_difference < 0)
        set_motors(max_speed+power_difference, max_speed);
    else
        set_motors(max_speed, max_speed-power_difference);
}

// A global ring buffer for data coming in.  This is used by the
// read_next_byte() and previous_byte() functions, below.
char buffer[100];

// A pointer to where we are reading from.
unsigned char read_index = 0;
int r=0;
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

// Backs up by one byte in the ring buffer.
void previous_byte()
{
    read_index --;
    if(read_index == 255)
        read_index = 99;
}

// Returns true if and only if the byte is a command byte (>= 0x80).
char is_command(char byte)
{
    if (byte < 0)
        return 1;
    return 0;
}

// Returns true if and only if the byte is a data byte (< 0x80).
char is_data(char byte)
{
    if (byte < 0)
        return 0;
    return 1;
}

// If it's not a data byte, beeps, backs up one, and returns true.
char check_data_byte(char byte)
{
    if(is_data(byte))
        return 0;

    play("o3c");

    clear();
    print("Bad data");
    lcd_goto_xy(0,1);
    print_hex_byte(byte);

    previous_byte();
    return 1;
}

/////////////////////////////////////////////////////////////////////
// COMMAND FUNCTIONS
//
// Each function in this section corresponds to a single serial
// command.  The functions are expected to do their own argument
// handling using read_next_byte() and check_data_byte().

// Sends the version of the slave code that is running.
// This function also shuts down the motors and disables PID, so it is
// useful as an initial command.
void send_signature()
{
    serial_send_blocking("3pi1.1", 6);
    set_motors(0,0);
    pid_enabled = 0;
}

// Reads the line sensors and sends their values.  This function can
// do either calibrated or uncalibrated readings.  When doing calibrated readings,
// it only performs a new reading if we are not in PID mode.  Otherwise, it sends
// the most recent result immediately.
void send_sensor_values(char calibrated)
{
    if(calibrated)
    {
        if(!pid_enabled)
            read_line_sensors_calibrated(sensors, IR_EMITTERS_ON);
    }
    else
        read_line_sensors(sensors, IR_EMITTERS_ON);
    serial_send_blocking((char *)sensors, 10);
}

// Sends the raw (uncalibrated) sensor values.
void send_raw_sensor_values()
{
    send_sensor_values(0);
}

// Sends the calibated sensor values.
void send_calibrated_sensor_values()
{
    send_sensor_values(1);
}

// Computes the position of a black line using the read_line()
// function, and sends the value.
// Returns the last value computed if PID is running.
void send_line_position()
{
    int message[1];
    unsigned int tmp_sensors[5];
    int line_position;

    if(pid_enabled)
        line_position = last_proportional+2000;
    else line_position = read_line(tmp_sensors, IR_EMITTERS_ON);

    message[0] = line_position;

    serial_send_blocking((char *)message, 2);
}

// Sends the trimpot value, 0-1023.
void send_trimpot()
{
    int message[1];
    message[0] = read_trimpot();
    serial_send_blocking((char *)message, 2);
}

// Sends the batter voltage in millivolts
void send_battery_millivolts()
{
    int message[1];
    message[0] = read_battery_millivolts();
    serial_send_blocking((char *)message, 2);
}

// Drives m1 forward.
void m1_forward()
{
    char byte = read_next_byte();
    
    if(check_data_byte(byte))
        return;

    set_m1_speed(byte == 127 ? 255 : byte*2);
}

// Drives m2 forward.
void m2_forward()
{
    char byte = read_next_byte();
    
    if(check_data_byte(byte))
        return;

    set_m2_speed(byte == 127 ? 255 : byte*2);
}

// Drives m1 backward.
void m1_backward()
{
    char byte = read_next_byte();
    
    if(check_data_byte(byte))
        return;

    set_m1_speed(byte == 127 ? -255 : -byte*2);
}

// Drives m2 backward.
void m2_backward()
{
    char byte = read_next_byte();
    
    if(check_data_byte(byte))
        return;

    set_m2_speed(byte == 127 ? -255 : -byte*2);
}

// A buffer to store the music that will play in the background.
char music_buffer[100];

// Plays a musical sequence.
void do_play()
{
    unsigned char tune_length = read_next_byte();

    if(check_data_byte(tune_length))
        return;

    unsigned char i;
    for(i=0;i<tune_length;i++)
    {
        if(i > sizeof(music_buffer)) // avoid overflow
            return;

        music_buffer[i] = read_next_byte();

        if(check_data_byte(music_buffer[i]))
            return;
    }

    // add the end of string character 0
    music_buffer[i] = 0;
    
    play(music_buffer);
}

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

// go into program space.
const char welcome_line1[] PROGMEM = " Pololu";
const char welcome_line2[] PROGMEM = "3\xf7 Robot";
const char demo_name_line1[] PROGMEM = "Line";
const char demo_name_line2[] PROGMEM = "follower";

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


void initialize()
{
    unsigned int counter; // used as a simple timer
    unsigned int sensors[5]; // an array to hold sensor values

    // This must be called at the beginning of 3pi code, to set up the
    // sensors.  We use a value of 2000 for the timeout, which
    // corresponds to 2000*0.4 us = 0.8 ms on our 20 MHz processor.
    pololu_3pi_init(2000);
    load_custom_characters(); // load the custom characters
    
    // Play welcome music and display a message
    print_from_program_space(welcome_line1);
    lcd_goto_xy(0,1);
    print_from_program_space(welcome_line2);
    play_from_program_space(welcome);
    delay_ms(1000);

    clear();
    print_from_program_space(demo_name_line1);
    lcd_goto_xy(0,1);
    print_from_program_space(demo_name_line2);
    delay_ms(1000);

    // Display battery voltage and wait for button press
    while(!button_is_pressed(BUTTON_B))
    {
        int bat = read_battery_millivolts();

        clear();
        print_long(bat);
        print("mV");
        lcd_goto_xy(0,1);
        print("Press B");

        delay_ms(100);
    }

    // Always wait for the button to be released so that 3pi doesn't
    // start moving until your hand is away from it.
    wait_for_button_release(BUTTON_B);
    delay_ms(1000);

    // Auto-calibration: turn right and left while calibrating the
    // sensors.
    for(counter=0;counter<80;counter++)
    {
        if(counter < 20 || counter >= 60)
        set_motors(40,-40);
        else
        set_motors(-40,40);

        // This function records a set of sensor readings and keeps
        // track of the minimum and maximum values encountered.  The
        // IR_EMITTERS_ON argument means that the IR LEDs will be
        // turned on during the reading, which is usually what you
        // want.
        calibrate_line_sensors(IR_EMITTERS_ON);

        // Since our counter runs to 80, the total delay will be
        // 80*20 = 1600 ms.
        delay_ms(20);
    }
    set_motors(0,0);

    // Display calibrated values as a bar graph.
    while(!button_is_pressed(BUTTON_B))
    {
        // Read the sensor values and get the position measurement.
        unsigned int position = read_line(sensors,IR_EMITTERS_ON);

        // Display the position measurement, which will go from 0
        // (when the leftmost sensor is over the line) to 4000 (when
        // the rightmost sensor is over the line) on the 3pi, along
        // with a bar graph of the sensor readings.  This allows you
        // to make sure the robot is ready to go.
        clear();
        print_long(position);
        lcd_goto_xy(0,1);
        display_readings(sensors);

        delay_ms(100);
    }
    wait_for_button_release(BUTTON_B);

    clear();

    print("Go!");

    // Play music and wait for it to finish before we start driving.
    play_from_program_space(go);
    while(is_playing());
}



// Clears the LCD
void do_clear()
{
    clear();
}


// Displays data to the screen
void do_print()
{
    unsigned char string_length = read_next_byte();
    
    if(check_data_byte(string_length))
        return;

    unsigned char i;
    for(i=0;i<string_length;i++)
    {
        unsigned char character;
        character = read_next_byte();

        if(check_data_byte(character))
            return;

        print_character(character);
    }
}

// Goes to the x,y coordinates on the lcd specified by the two data bytes
void do_lcd_goto_xy()
{
    unsigned char x = read_next_byte();
    if(check_data_byte(x))
        return;

    unsigned char y = read_next_byte();
    if(check_data_byte(y))
        return;

    lcd_goto_xy(x,y);
}

// Runs through an automatic calibration sequence
void auto_calibrate()
{
    time_reset();
    set_motors(60, -60);  
    while(get_ms() < 250)  
        calibrate_line_sensors(IR_EMITTERS_ON);  
    set_motors(-60, 60);  
    while(get_ms() < 750)  
        calibrate_line_sensors(IR_EMITTERS_ON);  
    set_motors(60, -60);  
    while(get_ms() < 1000)  
        calibrate_line_sensors(IR_EMITTERS_ON);  
    set_motors(0, 0); 
    
    serial_send_blocking("c",1); 
}

// Turns on PID according to the supplied PID constants
void set_pid()
{
    unsigned char constants[5];
    unsigned char i;
    for(i=0;i<5;i++)
    {
        constants[i] = read_next_byte();
        if(check_data_byte(constants[i]))
            return;
    }

    // make the max speed 2x of the first one, so that it can reach 255
    max_speed = (constants[0] == 127 ? 255 : constants[0]*2);

    // set the other parameters directly
    p_num = constants[1];
    p_den = constants[2];
    d_num = constants[3];
    d_den = constants[4];

    // enable pid
    pid_enabled = 1;
}

// Turns off PID
void stop_pid()
{
    set_motors(0,0);
    pid_enabled = 0;
}
int z=0;
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

void jsonDecode(char* message)
{
    const char s[2] = ":";
    
    messType = strtok(message, s);
    messCommand = strtok(NULL, s);
    int lastCommand = strlen(messCommand)-1;
    memmove(&messType[0], &messType[0 + 1], strlen(messType) - 0);
    memmove(&messCommand[lastCommand], &messCommand[lastCommand + 1], strlen(messCommand) - lastCommand);
}


const char* jsonEncode(char* messType, char* messCommand)
{
    int len = strlen(messType) + strlen(messCommand) +4;
    
    char target[len];
    //strcat(target,messType);
    snprintf( target, sizeof( target ), "{%s:%s}", messType, messCommand);
    sendMessage(target);
    //lcd_goto_xy(0,0);
    //print(target);
    return target;
}


void sendJson(char* messType, char* messCommand)
{
    int len = strlen(messType) + strlen(messCommand) +6;
    if (len > 8){
        char target[len];
        //strcat(target,messType);
        snprintf( target, sizeof( target ), "{%s:%s}", messType, messCommand);
        sendMessage(target);
        lcd_goto_xy(2,0);
        print(target);
    }
    lcd_goto_xy(0,0);
    print_long(len);
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
 int leng=0;
 char received[10] = "STOP";
 char sent[10] = "NOPE";
 int motorSpeed = 30;
 int x=0;
 int leftMessageSent = 0;
 int rightMessageSent = 0;
 int forwardMessageSent = 0;
 char* direction = "*R";
 int firstPoint = 1;
 
 long startTime, endTime;



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
	
	// Play welcome music and display a message
	print_from_program_space(welcome_line1);
	lcd_goto_xy(0,1);
	print_from_program_space(welcome_line2);
	play_from_program_space(welcome);
	delay_ms(1000);

	clear();
	print_from_program_space(demo_name_line1);
	lcd_goto_xy(0,1);
	print_from_program_space(demo_name_line2);
	delay_ms(1000);

	// Display battery voltage and wait for button press
	delay_ms(1000);

	// Auto-calibration: turn right and left while calibrating the
	// sensors.
	for(counter=0;counter<80;counter++)
	{
		if(counter < 20 || counter >= 60)
		set_motors(40,-40);
		else
		set_motors(-40,40);

		// This function records a set of sensor readings and keeps
		// track of the minimum and maximum values encountered.  The
		// IR_EMITTERS_ON argument means that the IR LEDs will be
		// turned on during the reading, which is usually what you
		// want.
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
	//initialize();
		
	
	//startTime = get_ms() ;
	
	
   /* while(1)
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
			lcd_goto_xy(3,1);
			print(received);
			sendMessage(position);
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
		
    
    }*/
}

