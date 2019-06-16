"use strict";
const fs = require('fs');
const { lstatSync, readdirSync } = require('fs');
const { join } = require('path');
const path = require('path');
const Discord = require('discord.js');
const config = require('./config.json');
const prefix = config.prefix;
const streamifier = require('streamifier');
//Constants
const bot = new Discord.Client();
const isDirectory = source => lstatSync(source).isDirectory();
const getDirectories = source => readdirSync(source).map(name => join(source, name)).filter(isDirectory);
const soundCollection = new Array();
const soundFolder = path.join(__dirname, "/audios_opus/");
const audioFolders = getDirectories(soundFolder);
//Classes
class SoundCollection {
    constructor(prefix, commands, sounds) {
        this.prefix = prefix;
        this.commands = commands;
        this.sounds = sounds;
        this.soundRange = 0;
    }
 
    load() {
        this.sounds.forEach(element => {
            this.soundRange += element.weight;
            element.load(this);
        });
    }

    //From https://developer.mozilla.org/pt-BR/docs/Web/JavaScript/Reference/Global_Objects/Math/random
    randomRange(min, max) {
        min = Math.ceil(min);
        max = Math.floor(max);
        return Math.floor(Math.random() * (max - min)) + min;
    }

    random() {
        var j = 0;
        var number = this.randomRange(0, this.soundRange);

        for(var i = 0; i < this.sounds.length; i++) {
            j += this.sounds[i].weight;
            if(number < j) {
                return this.sounds[i];
            }
        }
    }

    getSoundWithName(name) {
        var sound = null;
        for(var i = 0; i < this.sounds.length; i++) {
            if(this.sounds[i].name == name) {
                sound = this.sounds[i];
                break;
            }
        }
        if(sound == null) {
            return this.random();
        } else {
            return sound;
        }
    }
};

class Sound {
    constructor(name, weight) {
        this.name = name;
        this.weight = weight;
    }

    load(soundCollection) {
        //console.log("Loading " + this.name);
        this.buffer = fs.readFileSync(path.join(soundFolder,soundCollection.prefix, this.name + '.opus'));
        if(this.buffer.length == 0) {
            console.log(`Error on load ${this.name}`);
        }
    }
};

class Play {
    constructor(message, sound) {
        this.message = message;
        this.sound = sound;
    }

    getUser() {
        return this.message.author.username;
    }

    getUserId() {
        this.getUser().id;
    }
};

class Queue {
    constructor(client, maxQueue) {
        this.maxQueue = maxQueue;
        this.queues = {};    //queue[serverid]{}
        this.client = client;
    }

    getQueue(serverid) {
        if(!this.queues[serverid]) {
            this.queues[serverid] = [];
        }
        return this.queues[serverid];
    }

    add(message, sound) {
        const queue = this.getQueue(message.guild.id);
        if(queue.length >= this.maxQueue)
        {
            return message.channel.send("Queue Full");
        }
        queue.push(new Play(message, sound));
        if(queue.length === 1 || !(this.client.VoiceConnections.find(val => val.channel.guild.id == message.guild.id) === undefined)) {
            this.play(message, queue);
        }
    }

    remove(message) {
        const queue = this.getQueue(message.guild.id);
        queue.shift();
    }
    
    play(message, queue) {
        if(queue.length === 0) {
            var voiceConnection = this.client.voiceConnections.find(val => val.channel.guild.id == message.guild.id);
            if (voiceConnection !== undefined) return voiceConnection.disconnect();
        }

        new Promise((resolve, reject) => {
            var voiceConnection = this.client.voiceConnections.find(val => val.channel.guild.id == message.guild.id);
            
            if (voiceConnection === undefined) {
                // Check if the user is in a voice channel.
                if (message.member.voiceChannel && message.member.voiceChannel.joinable) {
                    message.member.voiceChannel.join()
                    .then(connection => {
                        resolve(connection);
                    })
                    .catch((error) => {
                        console.log(error);
                        reject();
                    });
                } else if (!message.member.voiceChannel.joinable) {
                    message.channel.send("I can't join in this Voice Channel.");
                    reject();
                } else {
                    // Otherwise, clear the queue and do nothing.
                    queue.splice(0, queue.length);
                    reject();
                }
            } else {
                resolve(voiceConnection);
            }
        })
        .then(connection => {
            var currentItem = queue[0];
            var currentSound = currentItem.sound;
            console.log(queue)
            console.log(`Playing ${currentSound.name} by user ${currentItem.getUser()}`);
            var stream = streamifier.createReadStream(currentSound.buffer);
            var audio = connection.playStream(stream);
            connection.on('error', (error) => {
                console.log(`Error on execute sound ${currentSound.name} by user ${currentItem.getUser()}`);
            });

            audio.on("end", end =>{
                setTimeout(()=> {   //Delay
                    if(queue.length > 0) {
                        this.remove(message);
                        this.play(message, queue);
                    }
                }, 1000);
            });
        }).catch(console.error);
    }
};
//
const queue = new Queue(bot, config.maxQueue);

//Main Program

//Load Audios
console.log(`Loading Audios...`);
audioFolders.forEach(element => {
    var folderName = path.basename(element);
    //console.log(folderName);

    var commands = new Array();
    commands.push(prefix + folderName);
    
    var sounds = new Array();
    fs.readdirSync(element).forEach(file => {
        var soundName = path.basename(file, '.opus');
        //console.log("   -" + soundName);
        sounds.push(new Sound(soundName.toLowerCase(), 1));
    });
    var sC = new SoundCollection(folderName, commands, sounds);
    sC.load();
    soundCollection.push(sC);
});
console.log(`Audios Loaded!`);

//buffer do audio
var audio_buffer = fs.readFileSync(path.join(__dirname,"/audios_opus/darksouls/good.opus"));

//Discord Client
bot.on("ready",function(){
    
    console.log(`Logged In as ${bot.user.tag}`);
    
    //Memory Usage
    const used = process.memoryUsage();
    for (let key in used) {
        console.log(`   ${key} ${Math.round(used[key] / 1024 / 1024 * 100) / 100} MB`);
    }
});

bot.on('message', (message) =>{
    var voiceChannel = message.member.voiceChannel;
    
    var parts = message.content.split(" ");

    soundCollection.forEach(c => {
        c.commands.forEach(command => {
            if(parts[0] == command) {
                switch(parts.length) {
                    case 1: {
                        queue.add(message, c.random());
                        /*voiceChannel.join()
                            .then(connection =>{
                                console.log("Conectou ");
                                var stream = streamifier.createReadStream(c.random().buffer);
                                var audio = connection.playStream(stream);
                                audio.on("start", ()=>{
                                    console.log("Tocou");
                                })
                                audio.on("end", end =>{
                                    voiceChannel.leave();
                                    console.log("Saiu");
                                })
                            })
                        .catch(console.error);*/
                    }
                    case 2: {
                        if(isNaN(parts[1])) {
                            voiceChannel.join()
                            .then(connection =>{
                                console.log("Conectou ");
                                var stream = streamifier.createReadStream(c.getSoundWithName(parts[1]).buffer);
                                var audio = connection.playStream(stream);
                                audio.on("start", ()=>{
                                    console.log("Tocou");
                                })
                                audio.on("end", end =>{
                                    voiceChannel.leave();
                                    console.log("Saiu");
                                })
                            })
                        .catch(console.error);
                        } else {
                            message.reply("Not Implemented Yet! :V");
                        }
                    }
                    case 3: {

                    }
                }
            }
        });
    });
});

bot.on('message', (message) =>{
    var voiceChannel = message.member.voiceChannel;
    if (message.content == "!teste") {
        voiceChannel.join()
            .then(connection =>{
                console.log("Conectou ");
                var stream = streamifier.createReadStream(audio_buffer);
                var audio = connection.playStream(stream);
                audio.on("start", ()=>{
                    console.log("Tocou");
                })
                audio.on("end", end =>{
                    voiceChannel.leave();
                    console.log("Saiu");
                })
            })
        .catch(console.error);
    }
    if(message.content == "!kill"){
        voiceChannel.leave();
    }

    if (message.content == "!teste2") {
        console.log(bot.voiceConnections);
        voiceChannel.join()
            .then(connection => {
                var buffer = fs.readFileSync(path.join(soundFolder, "carlosalberto" ,"chaos.opus"));
                var stream = streamifier.createReadStream(buffer);
                const som = connection.playStream(stream);
                console.log(bot.voiceConnections);
                som.on("end", end =>{
                    voiceChannel.leave();
                })
            })
        .catch(console.error);
    }

    //pergunta
    /*
        - quando mais pessoas fazem o comando ele "para"
        - precisa de fila
        - não apaga as mensagens
    */
    /*if(message.content[0] == prefix && message.content[message.content.length - 1] == "?"){
        var resp = Math.floor(Math.random() * 3);
        if(resp == 0){
            message.channel.sendMessage("Sim >//<");
        }else if(resp == 1){
            message.channel.sendMessage("Não '-'");
        }else{
            message.channel.sendMessage("Talvez -w-");
        }
    }*/

    /*
    if (message.content == "!black") {
        voiceChannel.join()
            .then(connection => {
                const som = connection.playFile(audios+"wacky_black.mp3");
                som.on("end", end =>{
                    voiceChannel.leave();
                })
            })
        .catch(console.error);
    }

     if (message.content == "!la") {
        voiceChannel.join()
            .then(connection => {
                const som = connection.playFile(audios+"la.mp3");
                som.on("end", end =>{
                    voiceChannel.leave();
                })
            })
        .catch(console.error);
    }

    if (message.content == "!max") {
        voiceChannel.join()
            .then(connection => {
                const som = connection.playFile(audios+"max.mp3");
                som.on("end", end =>{
                    voiceChannel.leave();
                })
            })
        .catch(console.error);
    }

    */

    //Random Link Img
    var arr = message.content.toLowerCase().split(" ");
    for (let i = 0; i < arr.length; i++) {
        if (arr[i] == "link") {
            var rand = SoundCollection.randomRange(0, 5);
            if (rand => 2 && rand <= 3) {
                message.reply("Aqui está http://www.smashbros.com/images/og/link.jpg");
            }
        }
    }

    //
});

bot.login(config.token);