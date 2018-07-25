long count = 0; 
long angle = 0;
volatile int A,B;
byte state, statep;
int last_angle = 0;
int angle_moved = 0;

void setup()
{
  Serial.begin(9600);//for debugging
  
  pinMode(4, INPUT);
  pinMode(5, INPUT);
  attachInterrupt(4,Achange,CHANGE);//interrupt pins are declared here
  attachInterrupt(5,Bchange,CHANGE);
}

void loop()
{
  Serial.println(count); 
  delay(200);
  
}

void Achange() 
{
  A = digitalRead(4);
  B = digitalRead(5);

  if ((A==HIGH)&&(B==HIGH)) state = 1;// switch...case method used here 
  if ((A==HIGH)&&(B==LOW)) state = 2;
  if ((A==LOW)&&(B==LOW)) state = 3;
  if((A==LOW)&&(B==HIGH)) state = 4;
  switch (state)
  {
    case 1:
    {
      if (statep == 2) count++;
      if (statep == 4) count--;
      break;
    }
    case 2:
    {
      if (statep == 1) count--;
      if (statep == 3) count++;
      break;
    }
    case 3:
    {
      if (statep == 2) count --;
      if (statep == 4) count ++;
      break;
    }
    default:
    {
      if (statep == 1) count++;
      if (statep == 3) count--;
    }
  }
  statep = state;
}

void Bchange()
{
  A = digitalRead(4);
  B = digitalRead(5);

  if ((A==HIGH)&&(B==HIGH)) state = 1;
  if ((A==HIGH)&&(B==LOW)) state = 2;
  if ((A==LOW)&&(B==LOW)) state = 3;
  if((A==LOW)&&(B==HIGH)) state = 4;
  switch (state)
  {
    case 1:
    {
      if (statep == 2) count++;
      if (statep == 4) count--;
      break;
    }
    case 2:
    {
      if (statep == 1) count--;
      if (statep == 3) count++;
      break;
    }
    case 3:
    {
      if (statep == 2) count --;
      if (statep == 4) count ++;
      break;
    }
    default:
    {
      if (statep == 1) count++;
      if (statep == 3) count--;
    }
  }
  statep = state;
}
