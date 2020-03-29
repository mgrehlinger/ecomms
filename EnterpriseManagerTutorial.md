- 1 ECOMMS Manager Home
   - 1 1 Synopsis
   - 1 2 Server
      - 1 2 1 Intro
      - 1 2 2 Getting started (server)
      - 1 2 3 Create our server
      - 1 2 4 Create our first feathersjs service
      - 1 2 5 Connecting to ECOMMS over nats.io
      - 1 2 6 Ask more questions
   - 1 3 Client
      - 1 3 1 Getting started (client)
      - 1 3 2 But first...
      - 1 3 3 Show list of instruments
      - 1 3 4 Sneak Peak!
   - 1 4 Meeting notes
      - 1 4 1 2019-02-13 Meeting notes
   - 1 5 Decision log
      - 1 5 1 another decision
      - 1 5 2 what is life


# ECOMMS Manager Home

```
Follow along as I create a web site that listens to ECOMMS and provide simple access to Instruments, Services, and Lab Equipment.
```
```
Follow along with the code on my Rpi here:
```
```
ssh pi@10.52.50.
```
```
raspberry ( password )
```
```
An advanced research app is here:
```
```
svn://10.53.240.28/Trios/users/sne/ECOMMS_VUE
```
## Recent space activity

```
Stephen Eshelman
another decision created Feb 13, 2019
```
```
Decision log updated Feb 13, 2019 • view change
```
```
what is life created Feb 13, 2019
```
```
Meeting notes updated Feb 13, 2019 • view change
```
```
2019-02-13 Meeting notes created Feb 13, 2019
```
## Space contributors

```
Stephen Eshelman (371 days ago)
```

# Synopsis

Develop simple single page web site that provides access to ECOMMS Participants like Instruments and Services using a javascript pipeline from
server to web client and utilizing Vue.js for the front-end. Access will include simple heartbeat monitoring ( knowing when and instrument or service is
available ), simple instrument interaction ( stop, start procedure, real time signals display, real time monitoring of running tests, view services and a listing
of a service's interface ).

The intent is to create a tool used during the development of ECOMMS ( shims, lab equipment interface, and services ) and to demonstrate how ECOMMS
can be made available to web clients utilizing a javascript pipeline.

Follow along as I create a web site that listens to ECOMMS and provide simple access to Instruments, Services, and Lab Equipment.

Follow along with the code on my Rpi here:

ssh pi@10.52.50.

raspberry ( password )

An advanced research app is here:

svn://10.53.240.28/Trios/users/sne/ECOMMS_VUE

( a snip of the latest wire frame )

( wire frame here unless there is a problem )


# Server


# Intro

This section is loosely labeled "server" as we will create a server that will run some feathersjs services that makes ecomms messages available to a
web page. We will develop a simple client towards the end that shows how this will be accomplished but we will mainly be talking about the server ( backe
nd ) side of things. We will first install the stuff required to get the ball rolling and then move into how to scaffold out a feathersjs app and create services.


# Getting started (server)

First install nodejs. Nodejs and the node package manager ( NPM ) will be used to get the rest of the libraries we will use to accomplish our task as
everything we will use is based on nodejs. I am installing and developing feon Windows, Mac, and Rpi. A great resource for getting nodejs onto an Rpi
can be found here ( see the bottom of the post ).

Next test you installation by entering the command node --version at the command line. You should see something like v11.4.0.

Next we need to scaffold a feathersjs app that will serve as our backend server. It's job will be to connect to ECOMMS( nats.io ) and make the Instruments
, Lab Equipment, and ECOMMS Services talking over ECOMMS available to our web clients.

To do so we need to install the feathers cli which is used to create a template feathersjs app. See here for more info.

To install the feathers cli type sudo npm install -g @feathersjs/cli. Test by issuing the command feathers. You should see something like this:

Usage: feathers generate [type]

Options:
-V, --version output the version number
-h, --help output usage information

Commands:
generate|g [type] Run a generator. Type can be

- app - Create a new Feathers application in the current folder
- authentication - Set up authentication for the current application
- connection - Initialize a new database connection
- hook - Create a new hook
- middleware - Create an Express middleware
- secret - Generate a new authentication secret
- service - Generate a new service
- plugin - Create a new Feathers plugin

upgrade|u Try to automatically upgrade to the latest Feathers version
*

Congratulations you're ready to go!


# Create our server

( for working code that's ahead of what we are doing checkout: svn://10.53.240.28/Trios/users/sne/ECOMMS_VUE )

Now it's time to use the feathers cli to create our feathersjs server app. This will serve as our gateway to ECOMMS and allow us to serve it up to a web
site.

Create a directory, name it ECOMMS_MANAGER, cd into this directory and create a directory call server.

Looks like this on my Rpi:

pi@raspberrypi_sne:~/dev/ECOMMS_MANAGER/server $

Now we'll fire up the feathers cli to create a feathersjs app.

Type feathers generate app.

Answer the questions as below then watch as the app is created (it will look as below):

pi@raspberrypi_sne:~/dev/ECOMMS_MANAGER/server $ feathers generate app
? Project name server
? Description feathers ecomms service server
? What folder should the source files live in? src
? Which package manager are you using (has to be installed globally)? npm
? What type of API are you making? (Press <space> to select, <a> to toggle all, <i> to invert selection)REST, Realtime via Socket.io
? Which testing framework do you prefer? Mocha + assert


```
create package.json
create config/default.json
create LICENSE
create public/favicon.ico
create public/index.html
create .editorconfig
create src/app.hooks.js
create src/channels.js
create src/index.js
create src/logger.js
create src/hooks/log.js
create src/middleware/index.js
create src/services/index.js
create .gitignore
create README.md
create src/app.js
create test/app.test.js
create .eslintrc.json
create config/production.json
No locked version found for winston@^3.0.0, installing latest.
No locked version found for nodemon, installing latest.
No locked version found for eslint, installing latest.
npm notice created a lockfile as package-lock.json. You should commit this file.
npm WARN server@0.0.0 No repository field.
npm WARN server@0.0.0 No license field.
+ serve-favicon@2.5.
+ @feathersjs/errors@3.3.
+ winston@3.1.
+ helmet@3.15.
+ cors@2.8.
+ @feathersjs/express@1.2.
+ @feathersjs/express@1.2.
+ @feathersjs/configuration@2.0.
+ @feathersjs/feathers@3.2.
+ compression@1.7.
+ @feathersjs/socketio@3.2.
added 166 packages from 102 contributors and audited 322 packages in 34.978s
found 0 vulnerabilities
npm WARN deprecated circular-json@0.3.3: CircularJSON is in maintenance only, flatted is its successor.
> nodemon@1.18.9 postinstall /home/pi/dev/ECOMMS_MANAGER/server/node_modules/nodemon
> node bin/postinstall || exit 0
Love nodemon? You can now support the project via the open collective:
> https://opencollective.com/nodemon/donate
npm WARN server@0.0.0 No repository field.
npm WARN server@0.0.0 No license field.
npm WARN optional SKIPPING OPTIONAL DEPENDENCY: fsevents@1.2.4 (node_modules/fsevents):
npm WARN notsup SKIPPING OPTIONAL DEPENDENCY: Unsupported platform for fsevents@1.2.4: wanted {"os":"darwin","
arch":"any"} (current: {"os":"linux","arch":"arm"})
+ request@2.88.
+ request-promise@4.2.
+ nodemon@1.18.
+ eslint@5.10.
+ mocha@5.2.
added 345 packages from 652 contributors and audited 2848 packages in 72.897s
found 0 vulnerabilities
```
Be sure to answer the API question with both REST and Realtime vis Socket.io as REST will be used during testing and Socket.io will be used to provide
ECOMMS to a web site.

Wait for the install to finish.

No start the server with npm start and visit [http://localhost:3030](http://localhost:3030) ( in my case the ip of the Rpi on my desk 10.52.50.59:3030 )

You will be greeted with:


Congratulations, you are well on your way to accessing ECOMMS from a web page!


# Create our first feathersjs service

12/18/2018 fixed links!

Next we need to create our first feathersjs service. This service will be used to listen to ECOMMS messages and make them accessible to a web site.

In the command shell type feathers generate service.

Answer the questions as below:

pi@raspberrypi_sne:~/dev/ECOMMS_MANAGER/server $ feathers generate service
? What kind of service is it? In Memory
? What is the name of the service? instruments
? Which path should the service be registered on? /instruments

Now start the server with npm start and visit [http://localhost:3030/instruments(](http://localhost:3030/instruments() or alternatively visit the Rpi at [http://10.52.50.59:3030/instruments).](http://10.52.50.59:3030/instruments).)

When the feathers app was created we stipulated REST and Socket.io as a result when we visit the /instruments page we will see:

{"total":0,"limit":10,"skip":0,"data":[]}

Now take a look at the files created. The file we are most concerned with is instruments.service.js in the src/services/instruments directory.

```
instruments.service.js
```
```
// Initializes the `instruments` service on path `/instruments`
const createService = require('feathers-memory');
const hooks = require('./instruments.hooks');
```
```
module.exports = function (app) {
```
```
const paginate = app.get('paginate');
```
```
const options = {
paginate
};
```
```
// Initialize our service with any options it requires
app.use('/instruments', createService(options));
```
```
// Get our initialized service so that we can register hooks
const service = app.service('instruments');
```
```
service.hooks(hooks);
};
```
Great we're on a way!


# Connecting to ECOMMS over nats.io

12/18/2018 fixed links!

In the last section Create our first feathersjs service we created a feathersjs service named instruments, started the ( npm start in the server directory)
feathersjs app ( server ) and visited the REST endpoint.

When we visit localhost:3030/instruments ( or alternatively [http://10.52.50.59:3030/instruments](http://10.52.50.59:3030/instruments) my Rpi) this is the output:

{"total":0,"limit":10,"skip":0,"data":[]}

Now we are going to update the output to be a list of the currently available ECOMMS instruments.

First we must add nats to our application.

Change directory to server and type npm install nats.

Open the package.json file in the root of the server directory and check that nats has been added to the dependencies:

"dependencies": {
"@feathersjs/configuration": "^2.0.4",
"@feathersjs/errors": "^3.3.4",
"@feathersjs/express": "^1.2.7",
"@feathersjs/feathers": "^3.2.3",
"@feathersjs/socketio": "^3.2.7",
"compression": "^1.7.3",
"cors": "^2.8.5",
"feathers-memory": "^2.2.0",
"helmet": "^3.15.0",
"nats": "^1.2.2",
"serve-favicon": "^2.5.0",
"winston": "^3.1.0"
},

Now open the server/src/services/instruments/instruments.service.js. We've seen this file in the last section:


```
instruments.service.js
```
```
// Initializes the `instruments` service on path `/instruments`
const createService = require('feathers-memory');
const hooks = require('./instruments.hooks');
```
```
var _instruments = {}; //added below
```
```
module.exports = function (app) {
```
```
const paginate = app.get('paginate');
```
```
const options = {
paginate
};
```
```
// Initialize our service with any options it requires
app.use('/instruments', createService(options));
```
```
// Get our initialized service so that we can register hooks
const service = app.service('instruments');
```
```
service.hooks(hooks);
};
```
We are going to add nats.io to it and listen for ECOMMS Instruments advertisements ( heartbeats ).

Up at the top of the file under the const hooks line add var _instruments = {}; and after the line service.hooks(hooks) line add:


```
instruments.service.js
```
### //NATS INTERFACE

```
var NATS = require('nats');
var nats = NATS.connect('nats://10.52.52.46:4222');
```
```
// Simple Subscriber
nats.subscribe('heartbeat', function(heartbeat) {
```
```
if(!(heartbeat in _instruments))
{
//get the participants role
nats.request(heartbeat + '.get', 'role', {'max':1},
function(role)
{
//if its an instrument create an entry in the instruments dictionary
if(role == 'Instrument')
{
_instruments[heartbeat] =
{
id:heartbeat
}
```
```
//then call create on the service
//to add it the instrument to the service store
```
```
service.create(_instruments[heartbeat]);
}
});
}
});
```
OK let's break it down.

We first add var _instruments = {}; to the top of the file. _instruments will initially be used to store instrument objects and to determine if an instrument
has been seen before.

Then we get the NATS library and connect to the nats broker running on aberdeen1 ( 10.52.52.46:4222 ) on the default port 4222.

```
instruments.service.js
```
### //NATS INTERFACE

```
var NATS = require('nats');
var nats = NATS.connect('nats://10.52.52.46:4222');
```
Then we start listening for heartbeats by subscribing to the heartbeat subject. When we get one we request the ECOMMS Participant's role. If the
Participant's role is Instrument we add it to the _instruments dictionary and create an instrument in the feathersjs service store passing the instrument
with service.create(_instruments[heartbeat];

Now visit localhost:3030/instruments ( or alternatively [http://10.52.50.59:3030/instruments](http://10.52.50.59:3030/instruments) my Rpi) again:

{"total":2,"limit":10,"skip":0,"data":[{"id":"d6ac139b-f63d-446c-88ba-fe798b2b2efb"},{"id":"2d7993af-e0fa-4181-
9467-171b7592a592"}]}

And you can see that there are two instruments ( the two thermal ECOMMS shims running on the RPi in my office ).



# Ask more questions

12/18/2018 fixed links!

OK we can see on localhost:3030 ( or alternatively [http:///10.52.50.59:3030/instruments](http:///10.52.50.59:3030/instruments) my Rpi) there are two instrument shims running:

{"total":2,"limit":10,"skip":0,"data":[{"id":"d6ac139b-f63d-446c-88ba-fe798b2b2efb"},{"id":"2d7993af-e0fa-4181-
9467-171b7592a592"}]}

But we don't know much information about them.

Let's ask them some more questions.

Let's go back to instruments.service.js in the src/services/instruments directory and add some more code.

Let's get the instrument's name, type, subtype, and location.

Replace the if block if(role == 'Instrument') with:


```
//if its an instrument get its name and location
//and create an entry in the instruments dictionary
if(role == 'Instrument')
{
_instruments[heartbeat] =
{
id:heartbeat,
lastAdvertisement: new Date(),
name: '---',
role: 'Instrument',
type: '---',
subtype: '---',
location: '---'
}
```
```
service.create(_instruments[heartbeat]);
```
```
//ask the instrument for its name
nats.request(heartbeat + '.get', 'name', {'max':1},
function(name)
{
console.log('name: ' + name);
```
```
_instruments[heartbeat].name = name;
```
```
service.update(heartbeat, _instruments[heartbeat]);
});
```
```
//ask the instrument for its type
nats.request(heartbeat + '.get', 'type', {'max':1},
function(type)
{
console.log('type: ' + type);
```
```
_instruments[heartbeat].type = type;
```
```
service.update(heartbeat, _instruments[heartbeat]);
});
```
```
//ask the instrument for its subtype
nats.request(heartbeat + '.get', 'subtype', {'max':1},
function(subtype)
{
console.log('subtype: ' + subtype);
```
```
_instruments[heartbeat].subtype = subtype;
```
```
service.update(heartbeat, _instruments[heartbeat]);
});
```
```
//ask the instrument for its location
nats.request(heartbeat + '.get', 'location', {'max':1},
function(location)
{
console.log('location: ' + location);
```
```
_instruments[heartbeat].location = location;
```
```
service.update(heartbeat, _instruments[heartbeat]);
});
}
```
Notice that once we have created the instrument in the service store with service.create( _instruments[heartbeat]); we then update the record in the
store with service.update(_instruments[heartbeat]); as the response for each request is received.


Now if you point a browser once again at localhost:3030 ( or alternatively [http:///10.52.50.59:3030/instruments](http:///10.52.50.59:3030/instruments) my
Rpi) there are two instrument shims running:

### {

```
"total": 2,
"limit": 10,
"skip": 0,
"data": [
{
"id": "e003a84f-2b8c-4b6b-9649-8a6377921377",
"lastAdvertisement": "2018-12-18T17:47:58.722Z",
"name": "DSC-Atlas",
"role": "Instrument",
"type": "Thermal",
"subtype": "None",
"location": "Lab"
},
{
"id": "2d7993af-e0fa-4181-9467-171b7592a592",
"lastAdvertisement": "2018-12-18T17:47:58.799Z",
"name": "dio2.0",
"role": "Instrument",
"type": "Thermal",
"subtype": "None",
"location": "heaven"
}
]
}
```
We're done with the server for the time being, let's get this list of instruments onto a web page...


# Client


# Getting started (client)

Ok now it's time to get a web page up that has access to our backend feathersjs services which are providing access to ECOMMS.

For this demonstration and moving forward I am using Vue.js for front-end development as I find it the most approachable after using React and Angular. V
ue.js has great documentation and getting a basic app up and running is a snap. The concepts are easily understood as well and routing and state
management ( Vuex ) are really easy to setup.

We are going to use the Vue CLI to create a basic, scaffolded app so we must first install it.

Type npm install -g @vue/cli on the command line. The Vue CLI gets installed globally, hence the -g option.

Assuming the install was successful ( I have had to add sudo to global installations when installing on my Rpi ), type vue --version and you should see: 3.
2.1.

Now we will create our first Vue.js application.

Change directory (cd in my case ) to the root of you ECOMMS_MANAGER project ( where the server directory is and in my case pi@raspberrypi_sne:~
/dev/ECOMMS_MANAGER $ ) and type

vue create client

When answering the questions choose: Babel, Router, Vuex, Yes ( this is my go to setup for prototyping ).

This took a couple of minutes on my Rpi so hang in there...

OK let's test the app creation, cd into the client directory and type npm run serve.

If all goes well the server will be started on port 8080 like so:

DONE Compiled successfully in 30358ms 1:15:58 PM

App running at:

- Local: [http://localhost:8080/](http://localhost:8080/)
- Network: [http://10.52.50.59:8080/](http://10.52.50.59:8080/)

Note that the development build is not optimized.
To create a production build, run npm run build.

Now visit [http://localhost:8080](http://localhost:8080) ( or click on the links above ) and you should be greeted by the web page:


Congratulations are in order as you are running your first Vue web app!

Next we will update our app to show a list of instruments that are currently talking over ECOMMS using our backend feathersjs service.


# But first...

We need a place to show our instruments list. Visit your ECOMMS_MANAGER folder and open the client folder in your favorite editor. This is the folder
that we used the Vue CLI to create the Vue.js application in way back. Visual Studio Code is great for this and I have been using Atom a lot lately. Both
are great and make it easy to run terminal commands while editing as both can run a terminal from within the editor. I have issues with running Code on
Mac OS X though and normally use Atom as a result. Start the web server and open the page by typing npm run serve in the client folder or visiting [http://](http://)
10.52.50.59:8080/ and if you're lucky the web server is running on my Rpi.

We will update the application created for us and:

```
create a page to display our instrument list
this page will have an instrument list component on it, create that component
create a route to the page
and add a link to the route on the main page next to the About like
```
Recall what that page looks like:

First go to /client/src/views and copy About.vue and name it Instruments.vue.

Now update Instruments.vue to look like this:

```
Instruments.vue
```
```
<template>
<div class="instruments">
<h1>WooHoo TA Instruments!</h1>
</div>
</template>
```
This is an html template tag and view uses templates to describe views and components.

Next open /src/router.js and add the line 4 and lines 17-21:


```
router.js
```
```
import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'
import Instruments from './views/Instruments.vue'
```
```
Vue.use(Router)
```
```
export default new Router({
mode: 'history',
base: process.env.BASE_URL,
routes: [
{
path: '/',
name: 'home',
component: Home
},
{
path: '/instruments',
name: 'instruments',
component: Instruments
},
{
path: '/about',
name: 'about',
// route level code-splitting
// this generates a separate chunk (about.[hash].js) for this route
// which is lazy-loaded when the route is visited.
component: () => import(/* webpackChunkName: "about" */ './views/About.vue')
}
]
})
```
This creates a route to our Instruments page.

Now what's left is to add a link to the main page to our new route: Open /src/App.vue and update the template adding the router-link tag <router-link to="
/instruments">Instruments</router-link> to the nav div.

```
App.vue
```
```
<template>
<div id="app">
<div id="nav">
<router-link to="/">Home</router-link> |
<router-link to="/about">About</router-link> |
<router-link to="/instruments">Instruments</router-link>
</div>
<router-view/>
</div>
</template>
```
Be sure to add the vertical bar to the line above as a separator.

Excellent! To see our handy work visit [http://10.52.50.59:8080/](http://10.52.50.59:8080/) or localhost:8080 if you are running locally and click Instruments in the nav bar at the top.


Next, use our feathersjs service to fetch a list of instruments available over ECOMMS!


# Show list of instruments

Ok we're ready to get a list of instruments from our backend featherjs server that is listening to ECOMMS. In order to do so we must add some stuff to our
client side app. Change directory into the client folder and type the following:

npm install --save @feathersjs/socketio-client socket.io-client @feathersjs/feathers. This will add the featherjs app and socket-io required
packages to the client app.

Next open the instruments view Instruments.vue and update it to look like this:

```
Instruments.vue
```
```
<template>
```
```
<div class="instruments">
<h1>WooHoo TA Instruments!</h1>
<instrument-list></instrument-list>
</div>
</template>
```
```
<script>
import InstrumentList from '@/components/InstrumentList.vue'
```
```
export default {
components:{
'instrument-list':InstrumentList
}
}
</script>
```
We have added a script tag and imported the not yet created component InstrumentList and used it in the template above. Data and components ( and
other things ) are exported in the script tag. Moving forward you will see the script tag and exports in all Vue views and components. Data, methods, and
components are describe in this export and is what makes Vue.js reactive.

For a quick look at the finished page visit [http://10.52.50.59:8080/instruments:](http://10.52.50.59:8080/instruments:)

Now let's create the InstrumentList component. In the client/src/components folder create InstrumentList.vue as below:


```
InstrumentList.vue
```
```
<template>
<div class="instrument-list">
```
```
<ul>
<li v-for="instrument in instruments">
{{ instrument.name }}@{{instrument.location}}
</li>
</ul>
```
```
</div>
</template>
```
```
<script>
const io = require('socket.io-client');
const feathers = require('@feathersjs/feathers');
const socketio = require('@feathersjs/socketio-client');
```
```
const socket = io('http://10.52.50.59:3030');
```
```
const client = feathers();
```
```
client.configure(socketio(socket));
```
```
const service = client.service('instruments');
```
```
export default {
mounted(){
service.find({}).then(result => {
this.instruments = result.data;
})
```
```
service.on('created', instrument => {
this.instruments.push(instrument)
})
},
```
```
data(){
return {instruments: []
}
}
}
</script>
```
Let's look at this component in three parts, the template, the feasthersjs service part of the script, and the exported stuff...

```
InstrumentList.vue
```
```
<template>
<div class="instrument-list">
```
```
<ul>
<li v-for="instrument in instruments">
{{ instrument.name }}@{{instrument.location}}
</li>
</ul>
```
```
</div>
</template>
```

In the template we are creating a simple un-ordered list ( <ul> ). Check out the <li> tag you'll find a v-for Vue.js directive. What this does is loops over
the instruments collection and creates an <li> for each item. The instrument collection is exported below from the data() function. In the list item ( <li> )
we use interpolation ( {{ value to be interpolated }} ) to display.

Next is the script tag and setup of the feathersjs app. The neat thing about feathersjs is that the setup is the same for the client and the server. Both sides
are represented by a feathersjs app.

```
InstrumentList.vue
```
```
const io = require('socket.io-client');
const feathers = require('@feathersjs/feathers');
const socketio = require('@feathersjs/socketio-client');
```
```
const socket = io('http://10.52.50.59:3030');
```
```
const client = feathers();
```
```
client.configure(socketio(socket));
```
```
const service = client.service('instruments');
```
Here we point the web socket at the feathersjs service running at 10.52.50.59:3030 ( my Rpi ) and get a reference to the service.

lastly we export the instruments list in data and then in the mounted() life cycle method we fetch the instrument list from the feathersjs service, update the
instruments list which triggers an update to the page.

```
InstrumentList.vue
```
```
export default {
mounted(){
service.find({}).then(result => {
this.instruments = result.data;
})
```
```
service.on('created', instrument => {
this.instruments.push(instrument)
})
},
```
```
data(){
return {instruments: []
}
}
```

# Sneak Peak!

Here is a look at ECOMMS_VUE...


# Meeting notes

```
Create meeting note
```
## Incomplete tasks from meetings

## Task report

Looking good, no incomplete tasks.

## All meeting notes

```
Title Creator Modified
```
```
2019-02-13 Meeting notes Stephen Eshelman Feb 13, 2019
```

# 2019-02-13 Meeting notes

## Date

13 Feb 2019

## Attendees

```
Stephen Eshelman
```
## Goals

## Discussion items

```
Time Item Who Notes
```
## Action items


# Decision log

```
Create decision
```
```
Decision Status Stakeholders Outcome Due date Owner
```
```
another decision NOT STARTED Stephen Eshelman
```
```
what is life NOT STARTED Stephen Eshelman
```

# another decision

```
Status NOT STARTED
```
```
Stakeholders
```
```
Outcome
```
```
Due date
```
```
Owner Stephen Eshelman^
```
## Background

## Action items


# what is life

```
Status NOT STARTED
```
```
Stakeholders
```
```
Outcome
```
```
Due date
```
```
Owner Stephen Eshelman^
```
## Background

## Action items


