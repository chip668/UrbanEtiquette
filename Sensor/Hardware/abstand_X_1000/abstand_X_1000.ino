typedef struct _SENSOR {
  String name;
  int trigger;
  int echo;
  int timeout;
  int duration;
  int distance;
  String dst;
  double faktor;
} SENSOR;

typedef SENSOR *PSENSOR;

SENSOR sensors [] = {
  {"rechts", D0, D1, 0, 0, 0.0, "", 1},
  {"links vorne", D5, D6, 0, 0, 0.0, "", 1},
  {"links hinten", D7, D8, 0, 0, 0.0, "", 0.75}
};

void initializeSensorPins(PSENSOR psensor) {
  pinMode(psensor->trigger, OUTPUT);
  pinMode(psensor->echo, INPUT);
}

void setup() {
  Serial.begin(9600);
  for (int i = 0; i < sizeof(sensors) / sizeof(sensors[0]); i++) {
    initializeSensorPins(&sensors[i]);
  }
}

void loop() {
  String text="";
  
  for (int i = 0; i < sizeof(sensors) / sizeof(sensors[0]); i++) {
    ReadDist(&sensors[i]);
    // Serial.println(sensors[i].name + " Entfernung: " + String (sensors[i].distance) + " cm");
    // text += ((char)i) + sensors[i].dst;
    Serial.print (String(sensors[i].distance) + ",");
  }
  Serial.println (text);
  delay(50);
}

void ReadDist(PSENSOR psensor) {
  int d = 0;
  digitalWrite(psensor->trigger, LOW);
  delayMicroseconds(2);
  digitalWrite(psensor->trigger, HIGH);
  delayMicroseconds(10);
  digitalWrite(psensor->trigger, LOW);
  if (psensor->timeout==0)
    d = pulseIn(psensor->echo, HIGH);
  else
    d = pulseIn(psensor->echo, HIGH, psensor->timeout);
  // Serial.println (String (psensor->trigger) + "/" + String (psensor->echo) + ": " + String(d));
  psensor->duration = d;
  psensor->distance = (int)(psensor->duration * 0.034 / 2);
  // psensor->dst = String((char)(psensor->distance*psensor->faktor));
  psensor->dst = (char)(psensor->distance * psensor->faktor);
}
