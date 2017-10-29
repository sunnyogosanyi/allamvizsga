#include <pololu/3pi.h>

void serial_slave_loop();

// If something is sent to the 3pi's RX jump into serial slave mode.
void check_for_serial_slave()
{
	if (serial_get_received_bytes() > 0)
	{
		serial_slave_loop();
	}
}

// PID constants
unsigned int pid_enabled = 0;
unsigned char max_speed = 255;
unsigned char p_num = 0;
unsigned char p_den = 0;
unsigned char d_num = 0;
unsigned char d_den = 0;
unsigned int last_proportional = 0;
unsigned int sensors[5];

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

// Waits for the next byte and returns it.  Runs play_check to keep
// the music playing and calls pid_check() to keep following the line.
char read_next_byte()
{
	while(serial_get_received_bytes() == read_index)
	{
		play_check();

		// pid_check takes some time; only run it if we don't have more bytes to process
		if(serial_get_received_bytes() == read_index)
			pid_check();
		
	}
	char ret = buffer[read_index];
	read_index ++;
	if(read_index >= 100)
		read_index = 0;
	return ret;
}


void send_data(char* data, long distance)
{
	int leng = -1;
	char buff[12];
	sprintf(buff,"%s-%d",data,distance);
	//strcat(message,(char*)distance);
	leng = strlen(buff);
	clear();
	print(buff);
	serial_send_blocking(buff, leng);
	delay_ms(2000);
}


// Sends the batter voltage in millivolts
void send_battery_millivolts()
{
	int message[1];
	message[0] = read_battery_millivolts();
	serial_send_blocking((char *)message, 2);
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

/////////////////////////////////////////////////////////////////////

void serial_slave_loop()
{	
	while(1)
	{
				
		// wait for a command
		char command = read_next_byte();
		
		// The list of commands is below: add your own simply by
		// choosing a command byte and introducing another case
		// statement.
		switch(command)
		{
		case (char)0x00:
			// silent error - probable master resetting
			break;

		case (char)0x61:
			send_data("L",150);
			break;
		
		case (char)0x62:
			send_data("R",120);
			break;

		case (char)0x63:
			send_data("R",160);
			break;

		case (char)0x64:
			send_data("L",200);
			break;
		case (char)0x0D:
			break;
		case (char)0x0A:
			break;

		default:
			clear();
			print("Bad cmd");
			lcd_goto_xy(0,1);
			print_hex_byte(command);

			play("o7l16c");
			continue; // bad command
		}
		
	}
}

// Initializes the 3pi serial module for serial slave mode.
void serial_initialize()
{
	set_digital_input(IO_D0, PULL_UP_ENABLED);
	// start receiving data at 115.2 kbaud
	serial_set_baud_rate(115200);
	serial_receive_ring(buffer, 100);
}

// Local Variables: **
// mode: C++ **
// c-basic-offset: 4 **
// tab-width: 4 **
// indent-tabs-mode: t **
// end: **
