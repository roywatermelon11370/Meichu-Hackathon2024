const express = require('express')
const SocketServer = require('ws').Server

const PORT = 8880;

const server = express().listen(PORT, () => console.log(`Listening on ${PORT}`));
const wss = new SocketServer({server})

wss.on('connection', ws =>{
    
    console.log('Client connected')

    ws.on('message', data =>{
        console.log(data.toString());
    })
    ws.on('close', () =>{
        console.log('Close Connected')
    })
})