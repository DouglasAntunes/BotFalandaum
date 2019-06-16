/*
 * To do list
 * 1 - Classe dos sons
 * 2 - Arrumar bug da stream
 * 3 - Sistema de Fila 
 */

// biblioteca
const Discord = require("discord.js");
// file system
const fs = require('fs');
// constante do bot
const bot = new Discord.Client();
// prefixo
const prefix = "!";
//lib de conversão buffer para readable stream
var streamifier = require('streamifier');

//buffer do audio
var audio_buffer = fs.readFileSync(__dirname + "/audios_opus/darksouls/good.opus");

bot.on("ready",function(){
    console.log("Prontinho mestre >////<");
});

bot.on('message', (message) =>{
    var voiceChannel = message.member.voiceChannel;
    console.log("Comandou");
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
bot.on('message', (message) =>{
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

bot.login("TOKEN");