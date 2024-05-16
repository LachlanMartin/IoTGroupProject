Module.register("MMM-sensordata", {
  defaults: {
  apiUrl: "http://localhost:5000/api/sensordata",
  conditionalsApiUrl: "http://localhost:5000/api/ConditionalRules",
  animationSpeed: 1000
  },
  start: function() {
    this.sensorData = {
        temperature: 0,
        lightLevel: 0
    };
    this.temperatureThreshold = 1000;
    this.updateInterval = 1000;
    this.fetchConditionalRules();
    this.scheduleUpdate();
},

scheduleUpdate: function() {
    if (this.updateTimer) {
      clearInterval(this.updateTimer);
    }
  
    this.updateTimer = setInterval(() => {
      this.fetchConditionalRules();
      this.getSensorData();
    }, this.updateInterval);
  },

fetchConditionalRules: function() {
    fetch(this.config.conditionalsApiUrl)
        .then(response => response.json())
        .then(data => {
            if (data.length > 0) {
                this.temperatureThreshold = data[0].temperatureThreshold;
                this.updateInterval = data[0].updateInterval || this.config.updateInterval;
                this.scheduleUpdate();
            }
        })
        .catch(error => {
            console.error("Error fetching conditional rules:", error);
        });
},

getSensorData: function() {
    fetch(this.config.apiUrl)
        .then(response => response.json())
        .then(data => {
            const latestData = data[data.length - 1];
            this.sensorData = {
                temperature: Math.round(latestData.temperature * 10) / 10,
                lightLevel: latestData.lightLevel
            };
            this.updateDom(this.config.animationSpeed);
            Log.info(`Sensor Data: ${JSON.stringify(this.sensorData)}`);

            // Check if temperature exceeds the threshold
            if (this.sensorData.temperature > this.temperatureThreshold + 10) {
                this.sendNotification("TEMPERATURE_ABNORMALLY_HIGH", {
                    temperature: this.sensorData.temperature
                });
            }
            else if (this.sensorData.temperature > this.temperatureThreshold) {
                this.sendNotification("TEMPERATURE_THRESHOLD_EXCEEDED", {
                    temperature: this.sensorData.temperature
                });
            }
        })
        .catch(error => {
            console.error("Error fetching sensor data:", error);
        });
},

getDom: function() {
    const wrapper = document.createElement("div");
    wrapper.classList.add("sensor-data");
  
    const temperatureElement = document.createElement("div");
    let temperatureAlert = "";
    if (this.sensorData.temperature > this.temperatureThreshold + 10) {
      temperatureAlert = "<p class='alert'>Temperature is abnormally high!</p>";
    } else if (this.sensorData.temperature > this.temperatureThreshold) {
      temperatureAlert = "<p class='alert'>Temperature threshold exceeded!</p>";
    }
    temperatureElement.innerHTML = `
      <p>Temperature: ${this.sensorData.temperature}°C</p>
      ${temperatureAlert}
    `;
    wrapper.appendChild(temperatureElement);
  
    const lightLevelElement = document.createElement("div");
    lightLevelElement.innerHTML = `
      <p>Light Level: ${this.sensorData.lightLevel}</p>
    `;
    wrapper.appendChild(lightLevelElement);
  
    const thresholdElement = document.createElement("div");
    thresholdElement.innerHTML = `
      <p>Temperature Threshold: ${this.temperatureThreshold}°C</p>
      <p>Update Interval: ${this.updateInterval / 1000} seconds</p>
    `;
    wrapper.appendChild(thresholdElement);
  
    return wrapper;
}, 

getStyles: function() {
    return [
        "MMM-sensordata.css"
    ];
},

notificationReceived: function(notification, payload, sender) {
    if (notification === "TEMPERATURE_THRESHOLD_CHANGED") {
        this.temperatureThreshold = payload.threshold;
    } else if (notification === "UPDATE_INTERVAL_CHANGED") {
        this.updateInterval = payload.interval;
        this.scheduleUpdate();
    }
}});