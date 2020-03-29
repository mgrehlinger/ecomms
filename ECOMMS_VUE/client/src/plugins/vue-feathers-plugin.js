import Vue from 'vue';

const io = require('socket.io-client');
const feathers = require('@feathersjs/feathers');
const socketio = require('@feathersjs/socketio-client');

const socket = io('http://192.168.86.127:3030');
const client = feathers();

client.configure(socketio(socket));

const vueFeathers = require('vue-feathers');

// And plug it in
Vue.use(vueFeathers, client);
