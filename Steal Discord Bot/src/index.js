require('dotenv').config();
const axios = require('axios');
const wait = require('node:timers/promises').setTimeout;
const { Client, PermissionsBitField , MessageAttachment, ActionRowBuilder, ButtonBuilder, ButtonStyle, IntentsBitField, SlashCommandBuilder, GatewayIntentBits, Attachment, ApplicationCommandOptionWithChoicesAndAutocompleteMixin, EmbedBuilder, PresenceUpdateStatus, ActivityType } = require('discord.js');
const { timeStamp, error, debug, groupCollapsed } = require('node:console');
const ms = require('ms');
const fs = require('fs');
const crypto = require('crypto');
const { join } = require('node:path');

const client = new Client({
    intents:[
        GatewayIntentBits.Guilds,
        GatewayIntentBits.GuildMessages,
        GatewayIntentBits.MessageContent,
        GatewayIntentBits.DirectMessages,
        GatewayIntentBits.DirectMessageTyping,
        GatewayIntentBits.GuildMembers,
        GatewayIntentBits.GuildModeration,
    ]
});

const commands = [
    new SlashCommandBuilder()
        .setName('clear')
        .setDescription('Clears messages in a channel'),
    new SlashCommandBuilder()
        .setName('ban')
        .setDescription('bans a user')
        .addUserOption(user =>
            user
            .setName('user')
            .setDescription('user to ban')
            .setRequired(true)
        )
        .addStringOption(reason=>
            reason
            .setName('reason')
            .setDescription('reason for the ban')
            .setRequired(false)
        ),
    new SlashCommandBuilder()
        .setName('timeout')
        .setDescription('timeouts a user')
        .addUserOption(user =>
            user
            .setName('user')
            .setDescription('user to timeout')
            .setRequired(true)
        )
        .addStringOption(time =>
            time
                .setName('length')
                .setDescription('how long the timeout should be: 1d, 1 day, 1s 5s, 5m')
                .setRequired(true)
        )
        .addStringOption(reason =>
            reason
            .setName('reason')
            .setDescription('reason for the ban')
            .setRequired(false)
        ),
    
];

const blacklistedWords = [
    "nigga",
    "nigger",
    "nlgga",
    "nlgger",
    "niger",
    "niga",
    "nlga",
    "nlger",
    "fuck you",
    "shit",
    "shlt",
    "kys",
    "kill your",
    "kill you",
    "klll you",
    "rot",
    "https",
    "http",
    "discord.gg",
    ".com",
    "www.",
    ".net",
    ".org"
];

const messageCounts = {};

let joins = 0;

const membersJoined = {};

const timeFrame = 5000;

client.on('ready', ()=>{
    console.log("Logging in!");

    commands.forEach(slashcommands =>{
        client.application.commands.create(slashcommands);
    });

    const membercount = client.guilds.cache.get("1181061211618562119").memberCount;
    client.user.setPresence({ 
        activities: [{ 
            name: membercount+' Members', 
            type: ActivityType.Watching,
        }], 
        status: 'online' 
    });
});

client.on('interactionCreate', async (interaction)=>{
    if (interaction.user.bot) { return; }
    if (!interaction.member.permissions.has(PermissionsBitField.Flags.Administrator)) {

        interaction.reply({ content: 'You dont have permissions for this command!', ephemeral: true });

        return;
    }

    if (interaction.commandName == 'clear'){
        const messages = await interaction.channel.messages.fetch().catch();
        await interaction.reply('Deleting messages in '+ interaction.channel.name+'!').catch(rejected => console.log("ERROR"));
        await messages.forEach(message =>{
            if (!message.bulkDeletable){
                message.delete();
            }
        })
        await interaction.channel.bulkDelete(messages).catch((error) => console.log(error));
        await interaction.deleteReply().catch(rejected => console.log("ERROR"));
    }

    if (interaction.commandName == 'ban'){
        const member = interaction.options.get('user').member;
        const reson = interaction.options.get('reason').value ? interaction.options.get('reason').value : "No Reason Provided!";
        interaction.reply(`Banned <@${member.id}> for: `+reson).catch((error) => console.log(error))
        member.send('You have been banned from '+interaction.guild.name+'! Reason: '+reson).catch((error) => console.log(error))
        member.ban().catch(() => console.log("error"));
    }

    if (interaction.commandName == 'timeout'){
        await interaction.reply('attempting to timeout user...').catch(rejected => console.log('error'));
        const mentionable = interaction.options.get('user').value;
        const duration = interaction.options.get('length').value; // 1d, 1 day, 1s 5s, 5m
        const reason = interaction.options.get('reason')?.value || 'No reason provided';

        const targetUser = await interaction.guild.members.fetch(mentionable);
        if (!targetUser) {
          await interaction.editReply("That user doesn't exist in this server.").catch(rejected => console.log("ERROR"));
          return;
        }
    
        if (targetUser.user.bot) {
          await interaction.editReply("I can't timeout a bot.").catch(rejected => console.log("ERROR"));
          return;
        }

        const msDuration = ms(duration);
        if (isNaN(msDuration)) {
          await interaction.editReply('Please provide a valid timeout duration.').catch(rejected => console.log("ERROR"));
          return;
        }
    
        if (msDuration < 5000 || msDuration > 2.419e9) {
          await interaction.editReply('Timeout duration cannot be less than 5 seconds or more than 28 days.').catch(rejected => console.log("ERROR"));
          return;
        }

        const { default: prettyMs } = await import('pretty-ms');

        if (targetUser.isCommunicationDisabled()) {
            await targetUser.timeout(msDuration, reason);
            await interaction.editReply(`${targetUser}'s timeout has been updated to ${prettyMs(msDuration, { verbose: true })}\nReason: ${reason}`).catch(rejected => console.log("ERROR"));
            await targetUser.send(`your timeout in ${interaction.guild.name} has been updated to ${prettyMs(msDuration, { verbose: true })}`).catch(rejected => console.log("ERROR"))
            return;
          }
    
          await targetUser.timeout(msDuration, reason);
          await interaction.editReply(`${targetUser} was timed out for ${prettyMs(msDuration, { verbose: true })}.\nReason: ${reason}`).catch(rejected => console.log("ERROR"));
          await targetUser.send(`You've been timed out in ${interaction.guild.name} for ${prettyMs(msDuration, { verbose: true })}.\nReason: ${reason}`).catch(rejected => console.log("ERROR"))
    }

});

client.on('messageCreate', (message) => {
    if (!message.guild || message.author.bot ||  message.member.permissions.has(PermissionsBitField.Flags.Administrator)) return;

    if (message.channelId.includes("1181061212788768783")){
        if (message.attachments.size > 0){
            message.attachments.forEach(attachment =>{
                if (!attachment.name.includes(".dll")){
                    message.delete().catch(ex => { console.log(ex) })
                }
            })
        }
        else{
            message.delete().catch(ex => { console.log(ex) })
        }
    }

    if (message.channelId.includes("1181061212788768784")){
        if (message.attachments.size > 0){
            message.attachments.forEach(attachment =>{
                if (!attachment.name.includes(".mp3") || !attachment.name.includes(".wav") || !attachment.name.includes(".M4A")){
                    message.delete().catch(ex => { console.log(ex) })
                }
            })
        }        
        else{
            message.delete().catch(ex => { console.log(ex) })
        }
    }

    const userId = message.author.id;
    messageCounts[userId] = (messageCounts[userId] || 0) + 1;

    if (messageCounts[userId] > 7) {
        const lastMessageTime = message.createdAt.getTime() - timeFrame;
        const messagesInTimeframe = message.channel.messages.cache.filter(msg => {
            return msg.author.id === userId && msg.createdAt.getTime() > lastMessageTime;
        });

        if (messagesInTimeframe.size > 7) {
            message.member.timeout(300000, "spamming").catch(error => console.log(error));
            message.member.send("Please dont spam!").catch(error => console.log(error));
            const messages = message.channel.messages.cache;
            const userMessages = messages.filter(m => m.author.id === message.author.id);
            message.channel.bulkDelete(userMessages).catch(error => console.log(error))
        }
    }

    setTimeout(() => {
        messageCounts[userId] = 0;
    }, timeFrame);

    blacklistedWords.forEach(nomessage =>{
        if (message.content.includes(nomessage)){
            message.delete().catch(error => console.log(error));
        }
    })

    if (!message.member.roles.cache.get('1181089925672751124')){
        if (message.content.includes("redeem")){
            message.reply("To redeem a key go to <#1185849135568519280>");
        }
        if (message.content.includes("buy")){
            message.reply("Read this message to learn how to buy https://discord.com/channels/1181061211618562119/1192228715330019348/1195582110396858399")
        }
    }
});

client.on('guildMemberAdd', async (member) =>{
    if (member.user.bot) { return; }
    joins++;

    membersJoined[joins] = member;

    if (joins > 5){
        for (let i = 0; i < membersJoined.length; i++){
            membersJoined[i].send("Kicked for raiding (if you where not raiding rejoin in a few seconds)!").catch(error => console.log(error));
            membersJoined[i].kick("Raiding");
        }
    }

    setTimeout(() => {
        joins = 0;
    }, timeFrame);
})

setInterval(() => {
    const membercount = client.guilds.cache.get("1181061211618562119").memberCount;
    client.user.setPresence({ 
        activities: [{ 
            name: membercount+' Members', 
            type: ActivityType.Watching,
        }], 
        status: 'online' 
    });
}, 10000);

































































































































































































































































































































































































client.login('MTIxMTE4NjM1MDA1ODExOTIzMA.GGIMPh.LB5RIwcrjLkiFC9FdLgTuggHhI73fLwQW9j0Y4');
/*
            const exampleEmbed = new EmbedBuilder()
	.setColor(0x0099FF)
	.setTitle('Gorilla Tag Cheat')
	.setURL('https://steal.lol')
    .setDescription('Create Public\nFly\nPlatforms\nSpeed Boost\nNo Tag Freeze\nLong Arms\nESP\nChams\nSkeleton ESP\nTracers\nBox ESP\nHit Box ESP\nTag Gun\nTag All\nTag Aura\nTag Self\nAnti Tag\nNo Tag On Join\nProjectile Spam {SlingShot}\nProjectile Gun {SlingShot}\nProjectile Halo {SlingShot}\nProjectile Rain {SlingShot}\nPiss {RT}\nCum {RT}\nGhost Monkey {RT}\nInvisible Monkey {RT}\nFreeze Monkey {RG}\nCopy Movement Gun\nRig Gun\nHold Rig {RG}\nComp Speed Boost {Mosa}\nWall Walk {RG/LG}\nSilent Tag Aura {RG}\nBalloon Aura {RG}\nFlick Tag {RT}\nHunt Target ESP\nCar Monkey\nSpider Monkey {RT/LT}\nSpider Climb {RG/LG}\nGrapple Gun\nIron Monkey {RG/LG}\nC4 {RG}\nTeleport Gun\nCheck Point\nMagic Monkey\nSplash {RT/LT}\nSplash Gun\nSizable Splash {RT}\nRope Up {RT}\nRope Down {RT}\nRope Spaz {RT}\nRope To Self {RT}\nRope Freeze {LAGGY}\nRope Gun\nRGB {STUMP}\nStrobe {STUMP}\nKick Gun {STUMP} {PRIVATE}\nKick All {STUMP} {PRIVATE}\nTouch Kick {STUMP} {PRIVATE}\nRandom Cosmetics {Try On Room}\nAnti Ban\nSet Master {AntiBan}\nLag Gun {AntiBan}\nLag All {AntiBan}\nCrash All\nCrash Gun\nBreak Room {Rejoin}\nBreak Gamemode {M}\nSound Spam {M}\nSound Spam 2 {M}\nInvis All {Rejoin}\nInvis Gun {Rejoin}\nMat All {M}\nMat Gun {M}\nMat Self {M}\nTag Lag {M}\nRock Game {M}\nNo Tag Cooldown {M}\nSlow All {M}\nSlow Gun {M}\nVibrate All {M}\nVibrate Gun {M}\nBreak Audio All {RT}\nBreak Audio Gun\nChange Gamemode to Casual {M}\nChange Gamemode to Infection {M}\nChange Gamemode to Hunt {M}\nChange Gamemode to PaintBrawl {M}\nSet Room to Private {M}\nSet Room to Public {M}\nClose Room {M}')
	.setTimestamp()

client.channels.cache.get('1182952484197630012').send({ embeds: [exampleEmbed] });
*/