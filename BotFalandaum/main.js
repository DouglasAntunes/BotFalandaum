/*
 * To do list
 * 1 - Sistema de Fila 
 */
const fs = require('fs');
const { lstatSync, readdirSync } = require('fs');
const { join } = require('path');
const path = require('path');
const Discord = require("discord.js");
const config = require("./config.json");
const prefix = config.prefix;
var streamifier = require('streamifier');
//Constants
const bot = new Discord.Client();
const isDirectory = source => lstatSync(source).isDirectory();
const getDirectories = source => readdirSync(source).map(name => join(source, name)).filter(isDirectory);
const soundCollection = new Array();
const soundFolder = path.join(__dirname, "/audios_opus/");
const audioFolders = getDirectories(soundFolder);
//Classes
class Play {

};

class Queue {

};

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
        console.log("Loading " + this.name);
        this.buffer = fs.readFileSync(path.join(soundFolder,soundCollection.prefix, this.name + '.opus'));
        if(this.buffer.length == 0) {
            console.log("Error on load " + this.name);
        }
    }
};

//
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

//buffer do audio
var audio_buffer = fs.readFileSync(path.join(__dirname,"/audios_opus/darksouls/good.opus"));

bot.on("ready",function(){
    console.log("Prontinho mestre >////<");
});

bot.on('message', (message) =>{
    var voiceChannel = message.member.voiceChannel;
    
    var parts = message.content.split(" ");

    soundCollection.forEach(c => {
        c.commands.forEach(command => {
            if(parts[0] == command) {
                switch(parts.length) {
                    case 1: {
                        voiceChannel.join()
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
                        .catch(console.error);
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
                            message.reply("Não implementado ^^");
                        }
                    }
                    case 3: {

                    }
                }
            }
        });
    });

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
});
//pergunta
/*bot.on('message', (message) =>{
    
    if(message.content[0] == prefix && message.content[message.content.length - 1] == "?"){
        var resp = Math.floor(Math.random() * 3);
        if(resp == 0){
            message.channel.sendMessage("Sim >//<");
        }else if(resp == 1){
            message.channel.sendMessage("Não '-'");
        }else{
            message.channel.sendMessage("Talvez -w-");
        }
    }
});
// quando mais pessoas fazem o comando ele "para"
// precisa de fila
//não apaga as mensagens
bot.on('message', (message) =>{
    var voiceChannel = message.member.voiceChannel;
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

});

bot.on('message', (message) =>{
    var voiceChannel = message.member.voiceChannel;
    if (message.content == "!zoom") {
        voiceChannel.join()
            .then(connection => {
                
                
                
            })
        .catch(console.error);
    }

});
bot.on('message', (message) =>{
    var voiceChannel = message.member.voiceChannel;
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

});
bot.on('message', (message) =>{
    var voiceChannel = message.member.voiceChannel;
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

});*/
bot.on('message', (message) =>{
    if (message.content == "!buf") {
       
    }
});
bot.on('message', (message) => {
    if(message.content == "!kill"){
        message.member.voiceChannel.leave();
    }
});

bot.on('message', (message) => {
    var arr = message.content.toLowerCase().split(" ");
    for (let i = 0; i < arr.length; i++) {
        if (arr[i] == "link") {
            message.reply("Aqui está http://www.smashbros.com/images/og/link.jpg");
        }
    }
});
/*  //Memory Usage
const used = process.memoryUsage();
for (let key in used) {
  console.log(`${key} ${Math.round(used[key] / 1024 / 1024 * 100) / 100} MB`);
}
*/
bot.login(config.token);