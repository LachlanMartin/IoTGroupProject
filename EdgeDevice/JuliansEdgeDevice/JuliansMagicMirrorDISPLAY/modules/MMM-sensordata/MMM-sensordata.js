Module.register("MMM-sensordata", {
    defaults: {
        apiUrl: "http://localhost:5000/api/sensordata/filtered",
        conditionalsApiUrl: "http://localhost:5000/api/sensordata/thresholds",
        animationSpeed: 1000,
        cardTapCheckInterval: 5000
    },
    start: function() {
        this.sensorData = {
            temperature: 0,
            lightLevel: 0
        };
        this.thresholds = {
            temperatureThreshold: 1000,
            lightLevelThreshold: 1000
        };
        this.cardTapped = false;
        this.updateInterval = 1000;
        this.fetchThresholds();
        this.scheduleUpdate();
    },

    scheduleUpdate: function() {
        if (this.updateTimer) {
            clearInterval(this.updateTimer);
        }

        this.updateTimer = setInterval(() => {
            this.fetchThresholds();
            this.getSensorData();
            this.checkCardTap();
        }, this.updateInterval);
    },

    fetchThresholds: function() {
        fetch(this.config.conditionalsApiUrl)
            .then(response => response.json())
            .then(data => {
                this.thresholds = data;
                this.updateDom(this.config.animationSpeed);
            })
            .catch(error => {
                console.error("Error fetching thresholds:", error);
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

                // Check if temperature or light level exceeds the threshold
                if (this.sensorData.temperature > this.thresholds.temperatureThreshold ||
                    this.sensorData.lightLevel > this.thresholds.lightLevelThreshold) {
                    this.sendNotification("THRESHOLD_EXCEEDED", {
                        temperature: this.sensorData.temperature,
                        lightLevel: this.sensorData.lightLevel
                    });
                }
            })
            .catch(error => {
                console.error("Error fetching sensor data:", error);
            });
    },

    checkCardTap: function() {
        // Logic to check if card is tapped in the last 5 seconds
        fetch(this.config.apiUrl)
            .then(response => response.json())
            .then(data => {
                const now = Date.now();
                const recentTap = data.find(d => d.cardUid && (now - new Date(d.timestamp).getTime() < this.config.cardTapCheckInterval));
                if (recentTap) {
                    this.cardTapped = true;
                    this.updateDom(this.config.animationSpeed);
                    setTimeout(() => {
                        this.cardTapped = false;
                        this.updateDom(this.config.animationSpeed);
                    }, this.config.cardTapCheckInterval);
                }
            })
            .catch(error => {
                console.error("Error checking card tap:", error);
            });
    },

    getDom: function() {
        const wrapper = document.createElement("div");
        wrapper.classList.add("sensor-data");

        const temperatureElement = document.createElement("div");
        let temperatureAlert = "";
        if (this.sensorData.temperature > this.thresholds.temperatureThreshold) {
            temperatureAlert = "<p class='alert'>Temperature threshold exceeded!</p>";
        }
        temperatureElement.innerHTML = `
            <p>Temperature: ${this.sensorData.temperature}°C</p>
            ${temperatureAlert}
        `;
        wrapper.appendChild(temperatureElement);

        const lightLevelElement = document.createElement("div");
        let lightLevelAlert = "";
        if (this.sensorData.lightLevel > this.thresholds.lightLevelThreshold) {
            lightLevelAlert = "<p class='alert'>Light level threshold exceeded!</p>";
        }
        lightLevelElement.innerHTML = `
            <p>Light Level: ${this.sensorData.lightLevel}</p>
            ${lightLevelAlert}
        `;
        wrapper.appendChild(lightLevelElement);

        const thresholdElement = document.createElement("div");
        thresholdElement.innerHTML = `
            <p>Temperature Threshold: ${this.thresholds.temperatureThreshold}°C</p>
            <p>Light Level Threshold: ${this.thresholds.lightLevelThreshold}</p>
            <p>Update Interval: ${this.updateInterval / 1000} seconds</p>
        `;
        wrapper.appendChild(thresholdElement);

        if (this.cardTapped) {
            const cardTapAlert = document.createElement("div");
            cardTapAlert.innerHTML = "<p class='alert'>Card tapped recently!</p>";
            wrapper.appendChild(cardTapAlert);
        }

        return wrapper;
    },

    getStyles: function() {
        return [
            "MMM-sensordata.css"
        ];
    },

    notificationReceived: function(notification, payload, sender) {
        if (notification === "TEMPERATURE_THRESHOLD_CHANGED") {
            this.thresholds.temperatureThreshold = payload.threshold;
        } else if (notification === "UPDATE_INTERVAL_CHANGED") {
            this.updateInterval = payload.interval;
            this.scheduleUpdate();
        }
    }
});
