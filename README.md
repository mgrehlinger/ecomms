- 1 Enterprise Communications over Nats.io or Every Instrument, Every Experiment, Everywhere!
   - 1 1 Overview
   - 1 2 discoverable, addressable, and identifiable!
   - 1 3 Can you hear me now?
   - 1 4 OOD and pub/sub communications
   - 1 5 Gets, Sets, Actions, and Status
   - 1 6 Participant Advertisements
   - 1 7 Gets, Sets, Actions, and Status - redux
   - 1 8 Ack/Nak/Response ( Request/Reply on steroids )
   - 1 9 One more thing! ( participant discovery and application development)
   - 1 10 ECOMMS Wire Protocol - subjects and behavior
      - 1 10 1 Terms and Conventions
      - 1 10 2 Heartbeat
      - 1 10 3 Get
      - 1 10 4 Set
      - 1 10 5 Action
      - 1 10 6 Status
      - 1 10 7 Event
      - 1 10 8 Participant
      - 1 10 9 Client
      - 1 10 10 Participant Process
      - 1 10 11 Client Process
      - 1 10 12 Manager
      - 1 10 13 Putting it all together
      - 1 10 14 Examples
      - 1 10 15 Questions/Observations/Enhancements
         - 1 10 15 1 Enhancements
         - 1 10 15 2 Observations
         - 1 10 15 3 Questions
   - 1 11 Creating experiments - keeping Client interface instrument agnostic and light weight
   - 1 12 Shim Participant development
   - 1 13 Instrument Client development
   - 1 14 Service Participant development
   - 1 15 Matrix Calculator, Oh Matrix Calculator...
   - 1 16 (WIP) Enterprise application development ( observing the manager and clients )
   - 1 17 ECOMMS Framework - Interface
   - 1 18 ECOMMS Framework - Participant
   - 1 19 ECOMMS Framework - Client
   - 1 20 Packages
   - 1 21 How to run demo DSC shim
   - 1 22 ECOMMS_VUE - Follow Along!
   - 1 23 possible with ecomms
      - 1 23 1 introduction
      - 1 23 2 address and role


# Enterprise Communications over Nats.io or Every

# Instrument, Every Experiment, Everywhere!

```
These pages describe an architecture and framework that provides enterprise visibility of analytical
instruments, lab equipment like balances and bar code readers, and software services over a common
communications pub/sub bus using nats.io.
```

# Overview

Today's laboratory can have any number of analytical instruments and lab equipment. In most cases the use of an analytical instrument and means of
data acquisition is facilitated by a direct connection usually over a local network or serial connection. This provides the connected client ( PC ) the means
to control the instrument while acquiring data. This kind of connection amounts to a private conversation with the instrument and has it's
limitations. Laboratory workflows can sometimes require collaborations between users, instruments, lab equipment, and software services.

The following pages discuss a work in progress protocol over Nats.io that provides the means for the kind of collaboration that would allow for example a
TGA experiment to automatically inform an experiment on a DSC, or allow the ability to create an adhoc workstation comprising a balance, bar code
reader, and analytical instrument, or for a software service that listens for all experiments on all instruments and store them to a persistent store or lims
system. The three tenets of good enterprise communications are discoverability, identification, and addressability. ECOMMS provide these tenets
and more.

# discoverable, addressable, and identifiable!

Discoverable, that is, what instruments, lab equipment, and ( software ) services, are available to do my bidding!
Addressable, "that balance over there!".
Identifiable, "what can you do for me?"


# Can you hear me now?

Collaborative communications necessarily presume that all participants are available, can be heard, and interacted with. This is different than our current
means of communicating with analytical instruments in which we generally have a direct, one way, connection which amounts to a private conversation.
Utilizing a pub/sub messaging server allows for the many-to-many, connectionless communications needed for truly collaborative and extensible
communications.

Let me explain.

Our model proposal comprises enterprise participants that have roles in our enterprise. Consider analytical instruments and lab equipment such as
balances and bar code readers as enterprise participants. Other enterprise participants are software services such as an analysis or persistent store
service. All enterprise participants communicate over a common communications bus that is a pub/sub message broker. Nats.io in our case. An example
of a software service participant is a persistent store service. This service participant is listening for experiments to start on any of the instruments that are
communicating over the bus. This service will 1) create an instance of an experiment in the persistent store 2) acquire experiment data.

The first step in getting all of the enterprise participants communicating over the same pub/sub message broker or bus and be able to discern between
them, all enterprise participants will communicate their presence by sending a regular participant advertisement ( think of it as a heartbeat ). This
advertisement provides the means to address the participant in further communications.

Consider a web client ( thin client ) that is presenting a page to a user with all ( those communicating over the enterprise bus ) available instruments. The
first step is to connect to the message broker ( or enterprise communications bus ) and listen for participant advertisements. Once an advertisement is
received it will be possible to use the information provided by the advertisement to address requests to the enterprise participant. There are questions that
all enterprise participants will be able to answer, one of which, "what is the role you provide to the enterprise", which is in our case is, "i am an instrument".


# OOD and pub/sub communications

A cornerstone of object oriented design is defining behaviors and where they go. That is, which class has which behavior. In our enterprise
communications there are common behaviors that all enterprise participants will have. The idea here is that there will be questions that all enterprise
participants will answer. Think of this as a participants base class. All enterprise participants will have a participant role. This is the role that the
enterprise participant plays in the enterprise. Example participant roles are: instrument, lab equipment, and service.

Participants will also have a type. Examples are: rheology, thermal, persistent store, analysis, balance, bar code reader, address generator. It is
the combination of participant role and type that completely defines the participant in the enterprise. If we consider the service participant role, one type
is persistent store. The persistent store service is responsible for creating experiment instances in a persistent store when any instrument participant
starts an experiment and begin acquiring data. It is also responsible for providing access to historical data.

This combination of participant role and type dovetails nicely in how the software abstractions for participants and their communications is designed.

Along with enterprise role and type all participants will be able to answer some other basic questions about itself like possibly user friendly name, location,
serial number, etc. From a library design perspective these items will be implemented in a participant base class ( discussed later ).


# Gets, Sets, Actions, and Status

While how you say something is important ( think enterprise communications over nats.io ), what you say is equally if not more so. For project success
and user take up it is important to have an understandable and concise protocol that easily moves from the communications bus to participant
implementation ( that is, translate the bus protocol into something that can be used to interface to an analytical instrument ).

The Mercury team devised a concise and simple way of categorizing communications with an instrument. Comms were split into gets, sets, actions, and
status. Each category has an expected client and instrument interaction. Giving a name and behavior to an element of an architecture makes to easy to
discuss how it fits in and define solutions in terms of those elements. Being able to think in terms of gets, sets, actions, and status was powerful and
enabling for the team. These terms will be carried over into our enterprise communications.

## gets

A get allows a client to request participant state information or data that it owns. For an instrument, think "get run state" ( answer, "i'm idle" ). For a
persistent store service, think "get experiment id's for experiments that contain <sne> in their name" ( answer, a list of experiment id's ). get requests
describe the what which is returned by the participant in its reply.

## sets

sets allow a client to update a participants state. sets contain the request ( the what ) and a payload ( the with ). For an instrument, think "set standby
temperature with 50 degrees C". set requests reply with "success" or "failure".

## actions

actions are a request for a participant to do something. For an instrument, think "start experiment". This request has no payload and is replied to by the
participant with a success for failure.

## status

status is a way to communicate to whomever is listening participant state on a regular basis. The Mercury communications also use a status to inform
connected clients that some state has changed ( action status ). The way this works is that every set request causes the instrument to send a set related
action status ( think set standby temperature and set temperature action status ) which then causes all connected clients to query the current state. This
concept will be carried over to our enterprise communications.


# Participant Advertisements

Consider this, you turn an instrument on and it begins advertising to the enterprise bus its presence. There may be a web page open, some lab
equipment talking, or no other participants at all. As participants can come and go it is important to understand 1) how to know something is there and
2) how to address requests to it. Once you have that then you can ask it what its role is in the enterprise and optionally what its name is and other
interesting information about it ( like location for instance ).

Participant advertisements will come on a regular basis ( somewhere between once every 3 seconds to once a second haven't decided yet ). An
advertisement comprises a subject that everyone knows about ( using heartbeat for now ) and a unique id that is used to address requests to this
participant. This is all that is needed to 1) how to know something is there and 2) how to address requests to it.

So what would that look like?

This is a sneak peek and not yet complete or nailed down!

Below is a snip of a running nats server. Don't try to understand everything that's going on just yet. There a couple of participants connecting to the
server ( an instrument sim and an address service ) and clients that are talking to them. The participant with all zeros is the address service. It
understands one request ( on top of the common participant requests ), nextaddress. You can see in the conversation below that a client requests the
next address which will be used to start the instrument sim participant. You can also see that when an instance of the instrument sim is created the first
thing it does is ask the participant for its role.

[26792] 2018/09/26 16:32:08.671905 [TRC] 127.0.0.1:58838 - cid:103 - ->> [CONNECT {"auth_token":null,"lang":".NET","name":null,"pass":null,"pedantic":
false,"protocol":1,"ssl_required":false,"user":null,"verbose":false,"version":"0.0.1"}]
[26792] 2018/09/26 16:32:08.671905 [TRC] 127.0.0.1:58838 - cid:103 - ->> [PING]
[26792] 2018/09/26 16:32:08.672405 [TRC] 127.0.0.1:58838 - cid:103 - <<- [PONG]
[26792] 2018/09/26 16:32:08.738953 [TRC] 127.0.0.1:58838 - cid:103 - ->> [SUB 00000000-0000-0000-0000-000000000000.get 1]
[26792] 2018/09/26 16:32:08.745458 [TRC] 127.0.0.1:58838 - cid:103 - ->> [SUB 00000000-0000-0000-0000-000000000000.set.* 2]
[26792] 2018/09/26 16:32:08.745458 [TRC] 127.0.0.1:58838 - cid:103 - ->> [SUB 00000000-0000-0000-0000-000000000000.action 3]
[26792] 2018/09/26 16:32:08.754964 [DBG] 127.0.0.1:58839 - cid:104 - Client connection created
[26792] 2018/09/26 16:32:08.759968 [TRC] 127.0.0.1:58839 - cid:104 - ->> [CONNECT {"auth_token":null,"lang":".NET","name":null,"pass":null,"pedantic":
false,"protocol":1,"ssl_required":false,"user":null,"verbose":false,"version":"0.0.1"}]
[26792] 2018/09/26 16:32:08.760469 [TRC] 127.0.0.1:58839 - cid:104 - ->> [PING]
[26792] 2018/09/26 16:32:08.760469 [TRC] 127.0.0.1:58839 - cid:104 - <<- [PONG]
[26792] 2018/09/26 16:32:08.774479 [TRC] 127.0.0.1:58838 - cid:103 - ->> [PUB heartbeat 36]
[26792] 2018/09/26 16:32:08.774479 [TRC] 127.0.0.1:58838 - cid:103 - ->> MSG_PAYLOAD: [00000000-0000-0000-0000-000000000000]
[26792] 2018/09/26 16:32:08.774979 [TRC] 127.0.0.1:58839 - cid:104 - ->> [SUB 00000000-0000-0000-0000-000000000000.status 1]
[26792] 2018/09/26 16:32:08.775480 [TRC] 127.0.0.1:58839 - cid:104 - ->> [SUB 00000000-0000-0000-0000-000000000000.status.* 2]
[26792] 2018/09/26 16:32:08.780983 [TRC] 127.0.0.1:58839 - cid:104 - ->> [SUB heartbeat 3]
[26792] 2018/09/26 16:32:08.792492 [TRC] 127.0.0.1:58839 - cid:104 - ->> [SUB _INBOX.468f88dd939d4f89ae33c3e64b982fe8.* 4]
[26792] 2018/09/26 16:32:08.792492 [TRC] 127.0.0.1:58839 - cid:104 - ->> [PUB 00000000-0000-0000-0000-000000000000.get _INBOX.
468f88dd939d4f89ae33c3e64b982fe8.0 4]
[26792] 2018/09/26 16:32:08.793492 [TRC] 127.0.0.1:58839 - cid:104 - ->> MSG_PAYLOAD: [role]
[26792] 2018/09/26 16:32:08.793993 [TRC] 127.0.0.1:58838 - cid:103 - <<- [MSG 00000000-0000-0000-0000-000000000000.get 1 _INBOX.
468f88dd939d4f89ae33c3e64b982fe8.0 4]
[26792] 2018/09/26 16:32:08.794494 [TRC] 127.0.0.1:58839 - cid:104 - ->> [PUB 00000000-0000-0000-0000-000000000000.get _INBOX.
468f88dd939d4f89ae33c3e64b982fe8.1 11]
[26792] 2018/09/26 16:32:08.794993 [TRC] 127.0.0.1:58839 - cid:104 - ->> MSG_PAYLOAD: [nextaddress]
[26792] 2018/09/26 16:32:08.795494 [TRC] 127.0.0.1:58838 - cid:103 - <<- [MSG 00000000-0000-0000-0000-000000000000.get 1 _INBOX.
468f88dd939d4f89ae33c3e64b982fe8.1 11]
[26792] 2018/09/26 16:32:08.799497 [TRC] 127.0.0.1:58839 - cid:104 - ->> [PING]
[26792] 2018/09/26 16:32:08.799997 [TRC] 127.0.0.1:58839 - cid:104 - <<- [PONG]
[26792] 2018/09/26 16:32:08.800497 [TRC] 127.0.0.1:58839 - cid:104 - ->> [PING]
[26792] 2018/09/26 16:32:08.800998 [TRC] 127.0.0.1:58839 - cid:104 - <<- [PONG]
[26792] 2018/09/26 16:32:08.811005 [TRC] 127.0.0.1:58838 - cid:103 - ->> [PUB _INBOX.468f88dd939d4f89ae33c3e64b982fe8.0 7]
[26792] 2018/09/26 16:32:08.812006 [TRC] 127.0.0.1:58838 - cid:103 - ->> MSG_PAYLOAD: [Service]
[26792] 2018/09/26 16:32:08.812506 [TRC] 127.0.0.1:58839 - cid:104 - <<- [MSG _INBOX.468f88dd939d4f89ae33c3e64b982fe8.0 4 7]
[26792] 2018/09/26 16:32:08.813007 [TRC] 127.0.0.1:58838 - cid:103 - ->> [PUB _INBOX.468f88dd939d4f89ae33c3e64b982fe8.1 36]
[26792] 2018/09/26 16:32:08.813507 [TRC] 127.0.0.1:58838 - cid:103 - ->> MSG_PAYLOAD: [ddca1e32-d41d-4c29-9b77-0c3ef8ffd133]
[26792] 2018/09/26 16:32:08.814007 [TRC] 127.0.0.1:58839 - cid:104 - <<- [MSG _INBOX.468f88dd939d4f89ae33c3e64b982fe8.1 4 36]
[26792] 2018/09/26 16:32:08.816509 [DBG] 127.0.0.1:58840 - cid:105 - Client connection created
[26792] 2018/09/26 16:32:08.820012 [TRC] 127.0.0.1:58840 - cid:105 - ->> [CONNECT {"auth_token":null,"lang":".NET","name":null,"pass":null,"pedantic":
false,"protocol":1,"ssl_required":false,"user":null,"verbose":false,"version":"0.0.1"}]
[26792] 2018/09/26 16:32:08.824014 [TRC] 127.0.0.1:58840 - cid:105 - ->> [PING]
[26792] 2018/09/26 16:32:08.827517 [TRC] 127.0.0.1:58840 - cid:105 - <<- [PONG]
[26792] 2018/09/26 16:32:08.843529 [TRC] 127.0.0.1:58840 - cid:105 - ->> [SUB ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.get 1]
[26792] 2018/09/26 16:32:08.844029 [TRC] 127.0.0.1:58840 - cid:105 - ->> [SUB ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.set.* 2]
[26792] 2018/09/26 16:32:08.844529 [TRC] 127.0.0.1:58840 - cid:105 - ->> [SUB ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.action 3]
[26792] 2018/09/26 16:32:08.848532 [TRC] 127.0.0.1:58840 - cid:105 - ->> [PUB heartbeat 36]
[26792] 2018/09/26 16:32:08.849032 [TRC] 127.0.0.1:58840 - cid:105 - ->> MSG_PAYLOAD: [ddca1e32-d41d-4c29-9b77-0c3ef8ffd133]
[26792] 2018/09/26 16:32:08.850033 [TRC] 127.0.0.1:58839 - cid:104 - <<- [MSG heartbeat 3 36]


[26792] 2018/09/26 16:32:08.854036 [DBG] 127.0.0.1:58841 - cid:106 - Client connection created
[26792] 2018/09/26 16:32:08.854537 [TRC] 127.0.0.1:58841 - cid:106 - ->> [CONNECT {"auth_token":null,"lang":".NET","name":null,"pass":null,"pedantic":
false,"protocol":1,"ssl_required":false,"user":null,"verbose":false,"version":"0.0.1"}]
[26792] 2018/09/26 16:32:08.854537 [TRC] 127.0.0.1:58841 - cid:106 - ->> [PING]
[26792] 2018/09/26 16:32:08.855037 [TRC] 127.0.0.1:58841 - cid:106 - <<- [PONG]
[26792] 2018/09/26 16:32:08.872550 [TRC] 127.0.0.1:58841 - cid:106 - ->> [SUB ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.status 1]
[26792] 2018/09/26 16:32:08.872550 [TRC] 127.0.0.1:58841 - cid:106 - ->> [SUB ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.status.* 2]
[26792] 2018/09/26 16:32:08.877054 [TRC] 127.0.0.1:58841 - cid:106 - ->> [SUB heartbeat 3]
[26792] 2018/09/26 16:32:08.877553 [TRC] 127.0.0.1:58841 - cid:106 - ->> [SUB _INBOX.5db830e23b18460cb1c309a06d02a62b.* 4]
[26792] 2018/09/26 16:32:08.878053 [TRC] 127.0.0.1:58841 - cid:106 - ->> [PUB ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.get _INBOX.
5db830e23b18460cb1c309a06d02a62b.0 4]
[26792] 2018/09/26 16:32:08.878554 [TRC] 127.0.0.1:58841 - cid:106 - ->> MSG_PAYLOAD: [role]
[26792] 2018/09/26 16:32:08.878554 [TRC] 127.0.0.1:58840 - cid:105 - <<- [MSG ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.get 1 _INBOX.
5db830e23b18460cb1c309a06d02a62b.0 4]
[26792] 2018/09/26 16:32:08.879055 [TRC] 127.0.0.1:58841 - cid:106 - ->> [PING]
[26792] 2018/09/26 16:32:08.879555 [TRC] 127.0.0.1:58841 - cid:106 - <<- [PONG]
[26792] 2018/09/26 16:32:08.882057 [TRC] 127.0.0.1:58840 - cid:105 - ->> [PUB _INBOX.5db830e23b18460cb1c309a06d02a62b.0 10]
[26792] 2018/09/26 16:32:08.882057 [TRC] 127.0.0.1:58840 - cid:105 - ->> MSG_PAYLOAD: [Instrument]
[26792] 2018/09/26 16:32:08.882557 [TRC] 127.0.0.1:58841 - cid:106 - <<- [MSG _INBOX.5db830e23b18460cb1c309a06d02a62b.0 4 10]
[26792] 2018/09/26 16:32:09.848071 [TRC] 127.0.0.1:58840 - cid:105 - ->> [PUB ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.status.runstate 4]
[26792] 2018/09/26 16:32:09.848071 [TRC] 127.0.0.1:58840 - cid:105 - ->> MSG_PAYLOAD: [Idle]
[26792] 2018/09/26 16:32:09.855078 [TRC] 127.0.0.1:58841 - cid:106 - <<- [MSG ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.status.runstate 2 4]
[26792] 2018/09/26 16:32:10.848185 [TRC] 127.0.0.1:58840 - cid:105 - ->> [PUB ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.status.runstate 4]
[26792] 2018/09/26 16:32:10.848682 [TRC] 127.0.0.1:58840 - cid:105 - ->> MSG_PAYLOAD: [Idle]
[26792] 2018/09/26 16:32:10.854188 [TRC] 127.0.0.1:58841 - cid:106 - <<- [MSG ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.status.runstate 2 4]
[26792] 2018/09/26 16:32:11.772969 [TRC] 127.0.0.1:58838 - cid:103 - ->> [PUB heartbeat 36]
[26792] 2018/09/26 16:32:11.772969 [TRC] 127.0.0.1:58838 - cid:103 - ->> MSG_PAYLOAD: [00000000-0000-0000-0000-000000000000]
[26792] 2018/09/26 16:32:11.779976 [TRC] 127.0.0.1:58839 - cid:104 - <<- [MSG heartbeat 3 36]
[26792] 2018/09/26 16:32:11.780950 [TRC] 127.0.0.1:58841 - cid:106 - <<- [MSG heartbeat 3 36]
[26792] 2018/09/26 16:32:11.846362 [TRC] 127.0.0.1:58840 - cid:105 - ->> [PUB heartbeat 36]
[26792] 2018/09/26 16:32:11.846362 [TRC] 127.0.0.1:58840 - cid:105 - ->> MSG_PAYLOAD: [ddca1e32-d41d-4c29-9b77-0c3ef8ffd133]
[26792] 2018/09/26 16:32:11.851361 [TRC] 127.0.0.1:58839 - cid:104 - <<- [MSG heartbeat 3 36]
[26792] 2018/09/26 16:32:11.852332 [TRC] 127.0.0.1:58841 - cid:106 - <<- [MSG heartbeat 3 36]
[26792] 2018/09/26 16:32:11.853836 [TRC] 127.0.0.1:58840 - cid:105 - ->> [PUB ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.status.runstate 4]
[26792] 2018/09/26 16:32:11.854835 [TRC] 127.0.0.1:58840 - cid:105 - ->> MSG_PAYLOAD: [Idle]
[26792] 2018/09/26 16:32:11.855335 [TRC] 127.0.0.1:58841 - cid:106 - <<- [MSG ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.status.runstate 2 4]
[26792] 2018/09/26 16:32:12.847819 [TRC] 127.0.0.1:58840 - cid:105 - ->> [PUB ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.status.runstate 4]
[26792] 2018/09/26 16:32:12.848326 [TRC] 127.0.0.1:58840 - cid:105 - ->> MSG_PAYLOAD: [Idle]
[26792] 2018/09/26 16:32:12.854855 [TRC] 127.0.0.1:58841 - cid:106 - <<- [MSG ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.status.runstate 2 4]
[26792] 2018/09/26 16:32:13.848674 [TRC] 127.0.0.1:58840 - cid:105 - ->> [PUB ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.status.runstate 4]
[26792] 2018/09/26 16:32:13.848674 [TRC] 127.0.0.1:58840 - cid:105 - ->> MSG_PAYLOAD: [Idle]
[26792] 2018/09/26 16:32:13.855679 [TRC] 127.0.0.1:58841 - cid:106 - <<- [MSG ddca1e32-d41d-4c29-9b77-0c3ef8ffd133.status.runstate 2 4]
[26792] 2018/09/26 16:32:14.626896 [DBG] 127.0.0.1:58838 - cid:103 - Client connection closed
[26792] 2018/09/26 16:32:14.627395 [DBG] 127.0.0.1:58839 - cid:104 - Client connection closed
[26792] 2018/09/26 16:32:14.631898 [DBG] 127.0.0.1:58841 - cid:106 - Client connection closed
[26792] 2018/09/26 16:32:14.631898 [DBG] 127.0.0.1:58840 - cid:105 - Client connection closed


# Gets, Sets, Actions, and Status - redux

In order to understand how communications are facilitated over nats.io ( a pub/sub message broker ) a small amount of understanding of how pub/sub
works and how we use these concepts to provide enterprise communications. In basic pub/sub communications an application publishes and subscribes
to a subject. Consider an instrument run state. In this scenario the instrument is publishing its run state on changes. In simplest terms this could be
accomplished by publishing to the "runstate" subject with a payload of "idle". nats.io messages are essentially just that; a subject and a payload. subject
is a string and payload is a byte array ( see nats.io concepts ). For TA Enterprise Communications though, this is not quite enough as we need to be able
to identity who is publishing.

That is where the Participant Advertisement comes in.

Instruments, services, and lab equipment will advertise their availability and presence on a regular basis. This advertisement will have the one thing we
need to send requests it, essentially its address.

Now that we have some small idea what happens in the nats.io layer of our enterprise communications and what it is used for, forget it!

The ECOMMS class library contains easy to use classes that provide the needed behavior to interact over our nats.io enterprise communications bus while
also providing common behavior for all clients and participants. What this means is that the gets, sets, actions, and status concepts are baked into the
client and participant class libraries.

In case it was not apparent up until now, communications on the TA Enterprise Communications bus is sort of like a conversation between a client and a
participant. Both sides of the conversation are required to get anything done. Participants advertise their presence and listen for requests and an
instance of a client is used to hold a conversation with it. The address is used to connect the two.


# Ack/Nak/Response ( Request/Reply on steroids )

Most Nats.io libraries ( there are version for C#, Java, JavaScript, Python, etc. ) and MQTT for that matter, support a concept they request/reply. These
are referred to as point-to-point messages and are supported in the libraries used to connect and use a nats.io and MQTT servers. I can see that
skeptical look on you face already! The idea of point-to-point messages ( and replies ) does not seem to jibe with the anarchy of pub/sub messaging. In
pub/sub everyone can hear everything. Anarchy! So how is the request/reply ( ie. a publisher sends a messages to a particular publisher and it in
turns answers directly to the publisher or requestor ) different?

It's simple really. The libraries that provide access to these pub/sub brokers, when a request is sent through the library, create a temporary mailbox ( a
subject ) and then subscribe to it. This temporary mailbox is part of the message that is sent to a recipient ( the replier in this case ). If a reply-to mailbox
is available the recipient directs its response to it instead of simply publishing. Of course that presupposes that we have a way to target a recipient
directly. The information we need to do that is part of the Participant advertisement ( essentially a heartbeat ).

Mercury instruments communications protocol include the idea of an ack and nak. Ack signifies that the request is understood and that the receiver will
endeavor to fulfill the request. The response to a request can be success or failure and include any requested data. So if I send a request to "place a
phone call" to an instrument it would nak with "i don't understand" and a request to "set your standby temperature with 25 degrees C" would be
ack'ed and then some time later, after the request has been successfully fulfilled, responded to with "success". It is desired that we can, when needed,
provide this behavior for a Participant.

So what must ECOMMS do to provide the same ack/nak/response protocol. Since request/reply is implemented in the libraries that provide access to the
broker server they cannot be extended to provide the ack/nak/response protocol. But the concepts used to provide request/reply can.

The ECOMMS framework provides this interface between Clients and Participants easily by using the same behavior the broker libraries use to provide the
request/reply protocol. Before a Client sends a request it creates a temporary mailbox as the broker libraries create for a request/reply request. This reply-
to mailbox is then used by the Participant to send an ack or nak and then send a response when required ( that is if the request was ack'ed ). All of this is
transparent to the Client and Participant developer. Clients are provided with an easy way to associate code for each of the possibilities with a callback for
acks, naks, and responses. The same is the case for Participants with base class calls the send an ack, nak, or response.

For complete example see the TestConsoleApp project in the ECOMMS solution available here:

svn://10.53.240.28/Trios/users/sne/TA Enterprise Communications

In this example I have created a "simulated" instrument participant by inheriting from InstrumentParticipant.

```
SimInstrumentParticipant
```
```
class SimInstrumentParticipant : InstrumentParticipant
{
}
```
The Participant in this case represents a proxy for an instrument and as a result can accept action requests. Our "simulated" instrument supports an autos
ampler and can perform the action openlid. There is a shortcut that allows a participant to listen to all requests for a given subject; a wildcard. This
shorthand is the method registerActionFacility. More detailed documentation for this method is elsewhere but you just need to know that it is shorthand
for a wildcard subscription and looks like this:


```
registerActionFacility
```
```
public override void init()
{
base.init();
//register action facility
//you are expected to fulfill ack/nak/response protocol
registerActionFacility("control.autosampler", (what, args) =>
{
switch(what)
{
case "openlid":
Console.WriteLine("*** asked to openlid!!!");
ack(args.Message);
Console.WriteLine("*** opening!!!");
Thread.Sleep(5000);
Console.WriteLine("*** done opening!!!");
replyTo(args.Message, "SUCCESS");
break;
}
});
}
```
In this code above the simulated instrument provides a callback that will be called for all actions that start with control.autosampler. In this case only
the openlid request is supported but you can see any autosampler request could be added. The way this works is that the request is acked by calling
calling through ack(), then do work ( sleeping above ), then call through replyTo with the response, in this case the string SUCCESS. At the moment the
callback is called with the command part of the subject ( in this case openlid, the what ), and MsgHandlerEventArg, which is a type returned from the Nats.
io broker library. This may change in the future. nak's are similar but for most cases ( that is that a derived Participant doesn't support the requested
action ) the base class will nak() for you.

Below is how a user facing client uses a Client to makes a request and proved a callback for acks, naks, and responses.

```
client
```
```
//use a manager obtained client reference to send the openlid request
//and provide an ack and response callback
client.doAction("control.autosampler.openlid", new ResponseCompletionAdapter(
ack: () => Console.WriteLine("control.autosampler.openlid got ACK"),
response: (bytes) => Console.WriteLine("control.autosampler.openlid got RESPONSE " + Encoding.
UTF8.GetString(bytes, 0, bytes.Length))
));
```
Here the user facing client code sends the openlid request using a client obtained from a Manager instance and provides an ack and response
callback. At the moment doAction is performed in another thread created by the ECOMMS framework. I expect that there will be a synchronous version
allowing the user facing client developer to create there own thread and perform an action.


# One more thing! ( participant discovery and application

# development)

Wait...

That's two things

Creating enterprise, user facing, applications will necessarily require a way to know what participants are available and what their state is and be kept
informed as to their state as it changes ( what is that DSC doing over there now, NOW, AND NOW ). Application developers ( the client side of the client
participant conversation ) will be dealing with clients for the most part ( classes that inherit from Client in the ECOMMS library ) but keeping track of the
comings and goings of participants ( a user has turned an instrument on how do I know it is now available ) would be prohibitive and frankly code that
would be ( or should be ) the same for all applications.

As a result the ECOMMS Class Library and Framework provides an easy way to monitor the ECOMMS Universal bus and keep informed of who's
chatting and available.

Every application ( instrument or device shim, user facing application, etc ) will have an instance of an ECOMMS Manager. The Manager will deliver client
instances as participants come online and keep your application apprised of what is going on the enterprise communications bus. The intent of this project
is that all application platforms that we plan to develop enterprise applications for will have a version of this library ( C#, Java, JavaScript, etc ). So all
applications will have the same ( basically ) boiler plate code for communicating with, and continuously being informed of the state of, enterprise
communications bus participants.

Read on!


# ECOMMS Wire Protocol - subjects and behavior

## Overview

What follows is a description of the messaging across a message broker used to facilitate the ECOMMS protocol that provides discoverability, addressa
bility, and identifiability ( not a word! ). This communication is based on a few simple primitives; Get, request something from an Entity; Set, to set an
aspect of an Entities state; Action, to tell an Entity to do something; Status, a message that is emitted by an Entity that describes an aspect of its current
state.

Another way to look at it is that Get/Set are used to set and get an Entities state and Actions are used to access and execute the behavior of an Entity. It's
very OO ( OOD and pub/sub communications )! The protocol itself makes it very easy to create abstractions for the Entities that are communicating as the
messages are easily routed to abstractions ( objects ) that represent them.

This document highlights the communications over the message broker in terms of subjects ( aka topics ) and payloads and it's main audience is library
developers that want to take the reference implementation and create a supported language variant ( like C++, python, or any of the functional languages
). It describes the format of the communications that allow applications and Participants to communicate with each other as well as the creation of a
framework ( TA Enterprise Communications ) to tie everything together and ease the development of such applications.

Discoverability: The trait what gives ECOMMS users the ability to discover what ECOMMS Participants are online and available to interact with.

Addressability: The means to communicate with Entities and to know which messages are associated with a particular Entity.

Identifiabilty: The ability to know what an Entity is and what its interface is.


# Terms and Conventions

## Terms

## ECOMMS Universal Communications bus

This is the combination of the ECOMMS Protocol and the message broker, that is, the broker and all of the entities that are available. The intent that is
meant to be conveyed is that all of the entities are speaking the same language ( the Protocol ) and are on the same instance of the broker ( which could
be clustered, ie. more that one daemon ).

## ECOMMS Protocol

The set of messages and arrangement of the message broker subjects, that facilitates communication between entities on the ECOMMS Universal
Communications Bus.

## ECOMMS Class Library and Framework

A library and guidance on how Participants and Clients communicate. A C# alpha reference library exists today and demonstrates how Participants and
client software can be made to communicate with each other. This reference implementation is available as a TacoCo nuget package.

## Unique Session Id

This is a string used in the construction of a subject that uniquely identifies a Participant for a given session with the broker. This id must be unique
among the current Participants connected to the broker but that is all. This unique id is used to facilitate communications with a Participant and is not
meant to identify the Participant or its role in a system. Each time a Participant connects to the message broker it will ( in most cases ) have a
different unique session id.

## Subject

A message broker message consists of a subject ( or topic ) and a payload. The subject is used to "route" the message to a recipient or recipients and will
normally be a dotted ( or some other token used as a separator ) hierarchy of tokens. For instance:

bde150bd-2e2f-415e-92be-ddaef12a971b.status.realtimesignals which is < unique session id >, a status, and the specific status.

## Payload

A message broker message consists of a subject ( or topic ) and a payload. In most cases brokers will treat this payload as a list of bytes. This leaves the
interpretation of the bytes up to the two side of the conversation; that is they must both agree on how to interpret the list of bytes. Many times a string will
suffice. A JSON string could be used to add structure or a Google Protocol Buffer could be used. All that is needed is for the Participant and Client to
agree as to how to interpret the payload.

## Entity

An Entity is anything that is available on the universal communications bus and using ECOMMS. These may be instruments, lab equipment, services,
instrument modules, etc.

## Participant

Think of the communication facilitated by ECOMMS as a conversation. Participants and Clients are two halves of the same conversation; a producer
and a consumer. An instrument is an example of a Participant. In code a Participant is the abstraction used to project an instrument onto the universal
communications bus;.

## Client

Client is the consumer side of the ECOMMS conversation. A client process will use Client instances to talk and listen to Participants.

## Participant Process

A Participant Process is a process that contains at least one Participant instance. We have talked extensively about instrument drivers ( shim ). An
instrument driver has an instance of a Participant that is configured through command line arguments or a configuration file to connect, natively, to an
instrument. The instrument is then projected onto the universal communications bus using the ECOMMS Protocol. This is referred to a virtual
instrument in that the instrument driver can and will provide functionality that may not be inherently supported by the instrument itself.


Client Process

A Client Process is any process that desires to interact with Participants on the universal communications bus. These can be anything from user facing
software ( ie. web browser ) to software services ( ie. persistent store service, schedule services, etc. ), to other Participants ( an instrument driver that is
collaborating with another instrument driver ).

Shim

This is any object ( Participant ) or process that connects natively to a device ( like an instrument, balance, scale, bar code reader, sensors, instrument
internal modules, etc. ) and acts as a proxy for the device on the ECOMMS Universal Communications bus.

Manager ( aka Manager )

Participant and Client processes will create an instance of Manager to monitor and obtain Client instances of all of the Participants available on the
universal bus.

Message Broker

Wikipedia: A message broker (also known as an integration broker or interface engine) is an intermediary computer program module that translates a
message from the formal messaging protocol of the sender to the formal messaging protocol of the receiver. Message brokers are elements in
telecommunication or computer networks where software applications communicate by exchanging formally-defined messages. Message brokers are a
building block of message-oriented middleware (MOM) but are typically not a replacement for traditional middleware like MOM and remote procedure call
(RPC).

NATS.io

NATS is a high performance messaging system that acts as a distributed messaging queue for cloud native applications, IoT device messaging, and
microservices architecture.

NATS.io Client Protocol

https://nats-io.github.io/docs/nats_protocol/nats-protocol.html

Subject ( see: https://nats-io.github.io/docs/developer/concepts/subjects.html )

NATS.io excerpt: "Subject-based Messaging Fundamentally NATS is about publishing and listening for messages. Both of these depend heavily on Subj
ects which scope messages into streams or topics. At its simplest, a subject is just a string of characters that form a name the publisher and subscriber
can use to find each other."

Conventions

When describing messages that are published the documentation will state the message in this way:

subject : payload

The subject is further broken down into:

< participant session id >. < ecomms primitive >. < optional what >

While different message brokers use different characters to separate the parts of a subject, in this documentation the period will be used.

The payload can be anything that both the Participant and Client agree on and is supported by the underlying message broker. For instance a Participant
status could be transmitted as a JSON string.


# Heartbeat

The heartbeat message is used to indicate the existence of a Participant and is emitted ( by a Participant ) on a regular basis ( the reference library is
emitting one of these every 3 seconds ). The heartbeat provides two things 1) discoverability and 2) addressability. A regular heartbeat indicates that
something ( an instrument driver, software service, etc. ) is alive and ready for interaction ( discoverability ). The heartbeat also provides the information
needed to interact with the Participant ( address ).

The heartbeat message consists of the subject heartbeat and a session unique id. This id must be unique for this session with the message broker (that
is all currently communicating Participants must have a unique heartbeat) becomes the current address for the Participant but has no other meaning. An
interested client upon sensing an available Participant will ask it questions that establish its identity and role in the system. Reference instrument driver
examples use a newly generated GUID for each session with the broker. This makes adhoc communications easy to achieve.

All messages directed to or emitted by a Participant use this address. This makes the development of a class library and communications framework
much easier as creating a software abstraction for a Participant is mostly done for us. Messages to and from a Participant are immediately identifiable (
they are all prefaced with this address as you will see later ).

## Subject : Payload

heartbeat : < session unique id >

## Example

```
Participant
```
```
var address = Guid.NewGuid().ToString();
```
```
SimInstrumentParticipant instrument = new SimInstrumentParticipant(address);
instrument.connect(@"nats://localhost:4222");
instrument.init();
```
### NATS:

[9856] 2019/08/20 11:10:32.519862 [[33mTRC[0m] 127.0.0.1:61598 - cid:579 - <<- [PUB heartbeat 36]
[9856] 2019/08/20 11:10:32.519862 [[33mTRC[0m] 127.0.0.1:61598 - cid:579 - <<- MSG_PAYLOAD: ["d6a8fa7b-4d4f-4684-a72a-4496c10485bc"]


# Get

The Get message is used by a Client to request something from a Participant. That something is the Participants ( whole or in part ) current state. This
could be static state like Particpants type or location or some current state like the current temperature. The subject is created from the Participant unique
session id and the get primitive and looks like this:

cf515783-e485-4aae-99c1-e4dd5db3b2d1.get

The payload is a string ( this may change as we move forward to JSON ) that represents WHAT is being requested ( like "location" ). These strings or lexic
on of the Participant will be agreed upon and documented for our known Participants ( virtual instruments ) but allow for adhoc communications between
applications and customer supplied equipment and software services.

## Subject : Payload

<unique session id>.get : <"WHAT">

Broker output:

[9856] 2019/08/27 10:09:05.434105 [[33mTRC[0m] 127.0.0.1:58245 - cid:596 - <<- [PUB cf515783-e485-4aae-99c1-e4dd5db3b2d1.get _INBOX.
28e30f93212149d89a701675dc6ed5a8.4 4]
[9856] 2019/08/27 10:09:05.434105 [[33mTRC[0m] 127.0.0.1:58245 - cid:596 - <<- MSG_PAYLOAD: ["location"]

## Extra

When a Client requests state information from a Participant it expects the response to be directed back at it. Frankly no other Entity on the universal
communications bus should even see the response. This behavior is accomplished by performing a request instead of a publish. A point to point means
of communications over a pub/sub message broker is accomplished using the concept of Request/Response. The gist of how this works is this:

```
a requester creates a temporary mailbox ( essentially a subject ) and subscribes to it
the request is send to the recipient with the temporary mailbox
the recipient check if a temporary mailbox is indicated and publishes the response to the temporary mailbox otherwise a normal publish is
performed
the requester receives the response through the temporary mailbox
the subscription to the temporary mailbox is removed
```
Most of the communications of ECOMMS takes advantage of the Request/Response idom. The ACK/NAK/RESPONSE CII idiom is also supported by
ECOMMS and uses the same Request/Response paradigm.

Request/Response is baked into the NATS wire protocol and an integral part of the broker client libraries but it is possible in all message brokers.

see: [http://www.steves-internet-guide.com/mqttv5-request-response/](http://www.steves-internet-guide.com/mqttv5-request-response/)

and:http://confluence.tainstruments.com/pages/viewpage.action?pageId=

## Response

The Participant sends its response to the reply-to subject specified by the Client ( or the broker client library ). This is the _INBOX subject in the PUB
message above.

Broker Output:

[9856] 2019/08/27 09:52:18.356697 [[33mTRC[0m] 127.0.0.1:57600 - cid:587 - <<- [PUB _INBOX.28e30f93212149d89a701675dc6ed5a8.4 6]
[9856] 2019/08/27 09:52:18.356697 [[33mTRC[0m] 127.0.0.1:57600 - cid:587 - <<- MSG_PAYLOAD: ["Heaven"]

## Example

```
Client Get
```
```
using System;
using System.Text;
using System.Threading;
```

using System.Threading.Tasks;
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Participant;
using NATS.Client;

namespace ConsoleApp
{
class DerivedParticipant : Participant
{
public DerivedParticipant(string address) : base(address, ECOMMS_Entity.Role.None, ECOMMS_Entity.Type.
None)
{
}

public override void get(string what, Msg message)
{
switch (what)
{
case "WHAT":
//get this through the instrument interface
ack(message);
replyTo(message, _what);
break;
default:
//not handled
base.get(what, message);
break;
}
}
}

class Program
{
static void Main(string[] args)
{
string address = Guid.NewGuid().ToString();

DerivedParticipant participant = new DerivedParticipant(address);
participant.connect("nats://localhost:4222");
participant.init();

Client client = new Client(address, Role.Equipment, ECOMMS_Entity.Type.None); ;
client.connect("nats://localhost:4222");
client.init();

client.doGet("WHAT", new ResponseCompletionAdapter(
ack: () =>
{
Console.WriteLine("responecompletionadapter ack");
},
nak: (error) =>
{
Console.WriteLine("responsecompletionadapter [get][location] : danger will robinson!");
},
response: (bytes) =>
{
Console.WriteLine("responsecompletionadapter [get][location] : " + Encoding.UTF8.GetString
(bytes, 0, bytes.Length));
we.Set();
}));

Console.WriteLine("Hello World!");

Console.CancelKeyPress += (o, e) =>
{
we.Set();
};

we.WaitOne();
}


### }

### }


# Set

The Set message is used by a Client to set some something in a Participant. That something is the Participants ( whole or in part ) current state. This
could be static state like a Particpants name or location or some current state like the current temperature. For instance a Client may need to set the
location of the Participant. The subject is created from the Participant unique session id, the set primitive, and the "WHAT", and looks like this:

cf515783-e485-4aae-99c1-e4dd5db3b2d1.set.location

The payload is a string ( this may change as we move forward to JSON ) that represents the new, requested, state.

## Subject:Payload

<unique session id>.set.<WHAT>:<"NEW STATE">

Broker output:

[9856] 2019/08/27 11:00:06.231128 [[33mTRC[0m] 127.0.0.1:59342 - cid:598 - <<- [PUB 7592a57e-d103-4dd1-8697-5b6fed33590c.set.location _INBOX.
70634170d6d04de9865abf9d964f39ea.5 6]
[9856] 2019/08/27 11:00:06.232128 [[33mTRC[0m] 127.0.0.1:59342 - cid:598 - <<- MSG_PAYLOAD: ["heaven"]

## Extra

As with most Client communications, a set is implemented as a Request ( Request/Reply idiom ) and as a result uses the brokers client library call for a
request.

## Response

The Participant sends its response to the reply-to subject specified by the Client ( or the broker client library ). This is the _INBOX subject in the PUB me
ssage above. In this case the response is "success" or "error".

## Example

```
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Participant;
using NATS.Client;
```
```
namespace ConsoleApp1
{
class DerivedParticipant : Participant
{
public DerivedParticipant(string address) : base(address, ECOMMS_Entity.Role.None, ECOMMS_Entity.Type.
None)
{
}
```
```
public override void get(string what, Msg message)
{
switch (what)
{
case "WHAT":
//get this through the instrument interface
ack(message);
replyTo(message, _what);
break;
default:
//not handled
base.get(what, message);
break;
}
}
```

string _what = "SOMETHING";
public override void set(string what, string payload, Msg message)
{
switch(what)
{
case "WHAT":
_what = payload;
replyTo(message, Encoding.ASCII.GetBytes("success"));
break;
default:
base.set(what, payload, message);
break;
}
}
}

class Program
{
static void Main(string[] args)
{
string address = Guid.NewGuid().ToString();

DerivedParticipant participant = new DerivedParticipant(address);
participant.connect("nats://localhost:4222");
participant.init();

Client client = new Client(address, Role.Equipment, ECOMMS_Entity.Type.None); ;
client.connect("nats://localhost:4222");
client.init();

//observe before settings
AutoResetEvent we = new AutoResetEvent(false);
client.doGet("WHAT", new ResponseCompletionAdapter(
ack: () =>
{
Console.WriteLine("responecompletionadapter ack");
},
nak: (error) =>
{
Console.WriteLine("responsecompletionadapter [get][location] : danger will robinson!");
},
response: (bytes) =>
{
Console.WriteLine("responsecompletionadapter [get][location] : " + Encoding.UTF8.GetString
(bytes, 0, bytes.Length));
we.Set();
}));
we.WaitOne();

//ensure set before observing
client.doSet("WHAT", "something else", (status) =>
{
Console.WriteLine(status);
we.Set();
});
we.WaitOne();

client.doGet("WHAT", new ResponseCompletionAdapter(
ack: () =>
{
Console.WriteLine("responecompletionadapter ack");
},
nak: (error) =>
{
Console.WriteLine("responsecompletionadapter [get][location] : danger will robinson!");
},
response: (bytes) =>
{
Console.WriteLine("responsecompletionadapter [get][location] : " + Encoding.UTF8.GetString
(bytes, 0, bytes.Length));
}));


Console.WriteLine("Hello World!");

Console.CancelKeyPress += (o, e) =>
{
we.Set();
};

we.WaitOne();
}
}
}


# Action

An Action is used by a Client to request an action be performed by a Participant. For instance to Start/Stop an experiment. Think of actions as the
behavior of the Participants, its methods. Its very OO ( OOD and pub/sub communications)! It was initially envisioned that an Action would not require
any additional data so the imperative is passed in the payload. This may change in the future. The subject is created from the Participant unique session
id and the action primitive and look like this:

e6c377a2-ef1c-4d8c-a82b-b9d2ef18679e.action

The payload is a string ( this may change as we move forward to JSON ) that represents the action to take.

## Subject:Payload

<unique session id>.action:<"ACTION">

Broker Output:

[9856] 2019/08/27 11:38:21.062501 [[33mTRC[0m] 127.0.0.1:60206 - cid:600 - <<- [PUB e6c377a2-ef1c-4d8c-a82b-b9d2ef18679e.action _INBOX.
7fac5bcb8a234535a1c299ec21825444.6 8]
[9856] 2019/08/27 11:38:21.063001 [[33mTRC[0m] 127.0.0.1:60206 - cid:600 - <<- MSG_PAYLOAD: ["THEACTION"]

## Extra

As with most Client communications, a set is implemented as a Request ( Request/Reply idiom ) and as a result uses the brokers client library call for a
request.

## Response

The Participant sends its response to the reply-to subject specified by the Client ( or the broker client library ). This is the _INBOX subject in the PUB me
ssage above. In this case the ACK/NAK/RESPONSE idiom is supported.

## Example

```
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Participant;
using NATS.Client;
```
```
namespace ConsoleApp1
{
class DerivedParticipant : Participant
{
public DerivedParticipant(string address) : base(address, ECOMMS_Entity.Role.None, ECOMMS_Entity.Type.
None)
{
}
```
```
public override void action(string what, Msg message)
{
switch(what)
{
case "DOACTION":
ack(message);
replyTo(message, Encoding.ASCII.GetBytes("success"));
break;
defaut:
base.action(what, message);
break;
```
```
}
base.action(what, message);
}
```

### }

class Program
{
static void Main(string[] args)
{
string address = Guid.NewGuid().ToString();

DerivedParticipant participant = new DerivedParticipant(address);
participant.connect("nats://localhost:4222");
participant.init();

Client client = new Client(address, Role.Equipment, ECOMMS_Entity.Type.None); ;
client.connect("nats://localhost:4222");
client.init();

client.doAction("DOACTION", new ResponseCompletionAdapter(
ack: () =>
{
Console.WriteLine("ACTION ack");
},
nak: (error) =>
{
Console.WriteLine("responsecompletionadapter [action] : danger will robinson!");
},
response: (bytes) =>
{
Console.WriteLine("ACTION RESPONSE [action] : " + Encoding.UTF8.GetString(bytes, 0, bytes.
Length));
}));

Console.WriteLine("Hello World!");

Console.CancelKeyPress += (o, e) =>
{
we.Set();
};

we.WaitOne();
}
}
}


# Status

The Status message is emitted by a Participant on a regular basis and a Participant can have any number of these depending on how it wants to report
its state or the state of subsystems it may manage. For instance real time signals are emitted as a status. The subject is created from the Participant
unique session id, the status primitive, and a status kind ( or status type ), and looks like this:

bde150bd-2e2f-415e-92be-ddaef12a971b.status.realtimesignals

The pay load can be a string, a JSON string, a list of bytes, or a buffer abstraction ( like a Google protocol buffer ).

## Subject:Payload

<unique session id>.status.<"status kind">:<string | list of bytes>

Broker Output:

[9856] 2019/08/27 15:01:40.589379 [[33mTRC[0m] 127.0.0.1:49301 - cid:684 - <<- [PUB bde150bd-2e2f-415e-92be-ddaef12a971b.status.KIND 6]
[9856] 2019/08/27 15:01:40.589379 [[33mTRC[0m] 127.0.0.1:49301 - cid:684 - <<- MSG_PAYLOAD: ["INFO14"]

## Extra

n/a

## Response

n/a

## Example


using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Participant;
using NATS.Client;

namespace ConsoleApp1
{
class DerivedParticipant : Participant
{
public DerivedParticipant(string address) : base(address, ECOMMS_Entity.Role.None, ECOMMS_Entity.Type.
None)
{
}

public override void init()
{
base.init();

Task.Factory.StartNew(() =>
{
int counter = 0;
while (true)
{
sendStatus("KIND", "INFO" + counter++);
Thread.Sleep(1000);
}
});
}
}

class Program
{
static void Main(string[] args)
{
string address = Guid.NewGuid().ToString();

DerivedParticipant participant = new DerivedParticipant(address);
participant.connect("nats://localhost:4222");
participant.init();

Client client = new Client(address, Role.Equipment, ECOMMS_Entity.Type.None); ;
client.connect("nats://localhost:4222");
client.init();

client.addStatusListener((status, data) =>
{
Console.WriteLine("{0}:{1}", status, ASCIIEncoding.ASCII.GetString(data));
});

Console.WriteLine("Hello World!");

Console.CancelKeyPress += (o, e) =>
{
we.Set();
};

we.WaitOne();
}
}
}


# Event

Events are emitted by Participants to inform all ECOMMS universal bus entities that something interesting has happened. For instance instrument
participants may raise an event each time an experiment starts. The subject is created from the Participant unique session id and the event primitive and
looks like this:

b05ab678-74d9-4db8-a4a2-67b149e837e5.event

The payload is a string ( this may change as we move forward to JSON ) that represents the event being raised.

## Subject:Payload

<unique session id>.event:<"EVENT">

Broker Output:

[9856] 2019/08/27 16:47:11.477313 [[33mTRC[0m] 127.0.0.1:59898 - cid:690 - <<- [PUB b05ab678-74d9-4db8-a4a2-67b149e837e5.event 30]
[9856] 2019/08/27 16:47:11.477313 [[33mTRC[0m] 127.0.0.1:59898 - cid:690 - <<- MSG_PAYLOAD: ["SOMETHING INTERESTING HAPPENED"]

## Extra

n/a

## Response

Clients are listening ( subscribed to their Participants events ) and notify observers of the event.

## Example


using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Participant;
using NATS.Client;

namespace ConsoleApp1
{
class DerivedParticipant : Participant
{
public DerivedParticipant(string address) : base(address, ECOMMS_Entity.Role.None, ECOMMS_Entity.Type.
None)
{
}

public override void init()
{
base.init();

Task.Factory.StartNew(() =>
{
while (true)
{
raise("SOMETHING INTERESTING HAPPENED");
Thread.Sleep(3000);
}
});

}
}

class Program
{
static void Main(string[] args)
{
string address = Guid.NewGuid().ToString();

DerivedParticipant participant = new DerivedParticipant(address);
participant.connect("nats://localhost:4222");
participant.init();

Client client = new Client(address, Role.Equipment, ECOMMS_Entity.Type.None); ;
client.connect("nats://localhost:4222");
client.init();

client.addObserver(new ObserverAdapterEx((observer, hint, data) =>
{
Console.WriteLine("client observer called: {0}", hint);
}));

//need to wait to not)
Console.WriteLine("Hello World!");

Console.CancelKeyPress += (o, e) =>
{
we.Set();
};

we.WaitOne();
}
}
}



# Participant

Entity : IObservable

Participant

Participant derived classes are created to provide proxies for devices ( like an instrument, balance, scale, bar code reader, sensors, instrument internal
modules, etc. ) and software services ( like persistent store, experiment data transformation, authorities like variables, etc. ); any device and software
service that is or could become a part of lab system.


# Client

Entity : IObservable

Client

Client instances are delivered by an instance of the Manager and generally are not created manually. Upon initialization of the Client ( init() ) Get
requests are performed to get the Participant name, role, type, and subtype. This happens in the base class Client. Then subscriptions are created
for status and event. This allows the Client to become a proxy for the Participant it is listening to. The Client then starts a heartbeat timer and starts
listening to the Participant heartbeat. The Client interface has the requisite methods to perform gets, sets, and actions.

## Interface

```
public interface IClient : IEntity
{
void init();
void doGet(string what, Action<string> callback);
void doGet(string what, IResponseCompletion callbacks);
void doSet(string what, string with, Action<string> callback);
void doSet(string what, byte[] with, Action<string> callback);
void doAction(string what, Action<string> callback);
void doAction(string what, IResponseCompletion callbacks);
void statusReceived(string name, byte[] data);
void addStatusListener(Action<string, byte[]> listener);
//indicate online/offline by listening to heartbeat
bool online { get; set; }
//void on(string anEventString, Action<string> callback);
}
```
## Example

```
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Participant;
using ECOMMS_Manager;
using NATS.Client;
using System.Linq;
```
```
namespace ConsoleApp1
{
class DerivedParticipant : Participant
{
public DerivedParticipant(string address) : base(address, ECOMMS_Entity.Role.Instrument, ECOMMS_Entity.
Type.None)
{
}
```
```
public override void init()
{
base.init();
```
```
Task.Factory.StartNew(() =>
{
int counter = 0;
while (true)
{
sendStatus("KIND", "INFO" + counter++);
Thread.Sleep(1000);
}
```

### });

Task.Factory.StartNew(() =>
{
while (true)
{
raise("SOMETHING INTERESTING HAPPENED");
Thread.Sleep(3000);
}
});

}

public override void get(string what, Msg message)
{
switch (what)
{
case "WHAT":
//get this through the instrument interface
ack(message);
replyTo(message, _what);
break;
default:
//not handled
base.get(what, message);
break;
}
}

string _what = "SOMETHING";
public override void set(string what, string payload, Msg message)
{
switch(what)
{
case "WHAT":
_what = payload;
replyTo(message, Encoding.ASCII.GetBytes("success"));
break;
default:
base.set(what, payload, message);
break;
}
}

public override void action(string what, Msg message)
{
switch(what)
{
case "DOACTION":
ack(message);
replyTo(message, Encoding.ASCII.GetBytes("success"));
break;
defaut:
base.action(what, message);
break;

}
base.action(what, message);
}
}

class Program
{
static void Main(string[] args)
{
string address = Guid.NewGuid().ToString();

DerivedParticipant participant = new DerivedParticipant(address);
participant.connect("nats://localhost:4222");
participant.init();


Client client = new Client(address, Role.Equipment, ECOMMS_Entity.Type.None); ;
client.connect("nats://localhost:4222");
client.init();

//observe before settings
AutoResetEvent we = new AutoResetEvent(false);
client.doGet("WHAT", new ResponseCompletionAdapter(
ack: () =>
{
Console.WriteLine("responecompletionadapter ack");
},
nak: (error) =>
{
Console.WriteLine("responsecompletionadapter [get][location] : danger will robinson!");
},
response: (bytes) =>
{
Console.WriteLine("responsecompletionadapter [get][location] : " + Encoding.UTF8.GetString
(bytes, 0, bytes.Length));
we.Set();
}));
we.WaitOne();

//ensure set before observing
client.doSet("WHAT", "something else", (status) =>
{
Console.WriteLine(status);
we.Set();
});
we.WaitOne();

client.doGet("WHAT", new ResponseCompletionAdapter(
ack: () =>
{
Console.WriteLine("GET ack");
},
nak: (error) =>
{
Console.WriteLine("responsecompletionadapter [get][location] : danger will robinson!");
},
response: (bytes) =>
{
Console.WriteLine("GET RESPONSE [get][location] : " + Encoding.UTF8.GetString(bytes, 0,
bytes.Length));
}));

client.doAction("DOACTION", new ResponseCompletionAdapter(
ack: () =>
{
Console.WriteLine("ACTION ack");
},
nak: (error) =>
{
Console.WriteLine("responsecompletionadapter [action] : danger will robinson!");
},
response: (bytes) =>
{
Console.WriteLine("ACTION RESPONSE [action] : " + Encoding.UTF8.GetString(bytes, 0, bytes.
Length));
}));

client.addStatusListener((status, data) =>
{
Console.WriteLine("{0}:{1}", status, ASCIIEncoding.ASCII.GetString(data));
});

client.addObserver(new ObserverAdapterEx((observer, hint, data) =>
{
Console.WriteLine("client observer called: {0}", hint);
}));


//need to wait to not)
Console.WriteLine("Hello World!");

Console.CancelKeyPress += (o, e) =>
{
we.Set();
};

we.WaitOne();
}
}
}


# Participant Process


# Client Process


# Manager

Entity : IObservable

Manager

An instance of a Manager ( ECOMMS_Manager.Manager ) is used both in a Participant process and Client process to monitor and interact with Participa
nts connected to the broker ( the ECOMMS Universal Communications bus ). A Manager instance manages a list of Clients; Clients that are currently
available (online) on the universal bus. Manager inherits from Entity and as a result is observable. A Manager instance connects to the broker and is
initialized in the same manner as Participants and Clients. The Manager instance is then observed for changes in its Client list. Manager creates
instances of Client for all discovered Participant's and listens to them. when a client goes offline the Client is removed from the list of Client's maintained
by the Manager

Manager instances can be created on an instance that implements the IClientFactory interface. This interface has one method, getClientFor(address,
role, type, subtype) and gives the Participant or Client Process the option to provide an object that can create derived Client instances.

## Interface

```
public interface IManager
{
IClient findClient(string withName);
IList<IClient> clients { get; }
}
```
## Example

```
Listen for client list changed
```

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Participant;
using ECOMMS_Manager;
using NATS.Client;
using System.Linq;

namespace ConsoleApp1
{
class DerivedParticipant : Participant
{
public DerivedParticipant(string address) : base(address, ECOMMS_Entity.Role.Instrument, ECOMMS_Entity.
Type.None)
{
}
}

class Program
{
static void Main(string[] args)
{
string address = Guid.NewGuid().ToString();

Manager manager = new Manager();
manager.connect("nats://localhost:4222");
manager.init();

manager.addObserver(new ObserverAdapter((o, h) =>
{
//need to wait to notify until after base class has gotton response
//to role request
//or have library query first before creating client
//WIP...

Thread.Sleep(1000);
switch (h)
{
case "CLIENTS_CHANGED":
Console.WriteLine(
"there are " +
manager.clients.Where((i) => i.role == Role.Instrument).Count() +
" instruments online"
);
break;
}

}));

}));
DerivedParticipant participant = new DerivedParticipant(address);
participant.connect("nats://localhost:4222");
participant.init();

//need to wait to not)
Console.WriteLine("Hello World!");

Console.CancelKeyPress += (o, e) =>
{
we.Set();
};

we.WaitOne();
}
}
}


Listen for connected Participants

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Participant;
using ECOMMS_Manager;
using NATS.Client;
using System.Linq;

namespace ConsoleApp1
{
class DerivedParticipant : Participant
{
public DerivedParticipant(string address) : base(address, ECOMMS_Entity.Role.Instrument, ECOMMS_Entity.
Type.None)
{
}

public override void init()
{
base.init();

Task.Factory.StartNew(() =>
{
int counter = 0;
while (true)
{
sendStatus("KIND", "INFO" + counter++);
Thread.Sleep(1000);
}
});
}
}

class Program
{
static void Main(string[] args)
{
string address = Guid.NewGuid().ToString();

Manager manager = new Manager();
manager.connect("nats://localhost:4222");
manager.init();

manager.addObserver(new ObserverAdapterEx((o, h, c) =>
{
//need to wait to notify until after base class has gotton response
//to role request
//or have library query first before creating client
//WIP...

var newClient = c as IClient;
Thread.Sleep(1000);
switch (h)
{
case "CONNECTED":
//was it an instrument?
if (newClient.role == Role.Instrument)
{
Console.WriteLine(newClient.name + " INSTRUMENT CONNECTED");

//listen for run state changes
newClient.addObserver(new ObserverAdapterEx((anobject, ahint, data) =>


### {

var bytes = (byte[])data;
var anInstrumentClient = (InstrumentClient)anobject;

if ((ahint as string) == "RUNSTATE_CHANGED")
{
var say = string.Format("{0} notified {1} with {2}",
newClient.name,
ahint,
Encoding.UTF8.GetString(bytes, 0, bytes.Length)
);

Console.WriteLine(say);
}
}));

//add a status listener
newClient.addStatusListener((name, data) =>
{
Console.WriteLine("status listener: {0}", name);
});
}
break;
}

}));

DerivedParticipant participant = new DerivedParticipant(address);
participant.connect("nats://localhost:4222");
participant.init();

//need to wait to not)
Console.WriteLine("Hello World!");

Console.CancelKeyPress += (o, e) =>
{
we.Set();
};

we.WaitOne();
}
}
}


# Putting it all together

Instrument Driver ( shim ) development

Service Participant development

ECOMMS Manager Home

Ack/Nak/Response ( Request/Reply on steroids )


# Examples

```
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Participant;
using NATS.Client;
```
```
namespace ConsoleApp1
{
class DerivedParticipant : Participant
{
public DerivedParticipant(string address) : base(address, ECOMMS_Entity.Role.None, ECOMMS_Entity.Type.
None)
{
}
```
```
public override void init()
{
base.init();
```
```
Task.Factory.StartNew(() =>
{
int counter = 0;
while (true)
{
sendStatus("KIND", "INFO" + counter++);
Thread.Sleep(1000);
}
});
}
```
```
public override void get(string what, Msg message)
{
switch (what)
{
case "WHAT":
//get this through the instrument interface
ack(message);
replyTo(message, _what);
break;
default:
//not handled
base.get(what, message);
break;
}
}
```
```
string _what = "SOMETHING";
public override void set(string what, string payload, Msg message)
{
switch(what)
{
case "WHAT":
_what = payload;
replyTo(message, Encoding.ASCII.GetBytes("success"));
break;
default:
base.set(what, payload, message);
break;
}
}
```
```
public override void action(string what, Msg message)
{
switch(what)
```

### {

case "DOACTION":
ack(message);
replyTo(message, Encoding.ASCII.GetBytes("success"));
break;
defaut:
base.action(what, message);
break;

}
base.action(what, message);
}
}

class Program
{
static void Main(string[] args)
{
string address = Guid.NewGuid().ToString();

DerivedParticipant participant = new DerivedParticipant(address);
participant.connect("nats://localhost:4222");
participant.init();

Client client = new Client(address, Role.Equipment, ECOMMS_Entity.Type.None); ;
client.connect("nats://localhost:4222");
client.init();

//observe before settings
AutoResetEvent we = new AutoResetEvent(false);
client.doGet("WHAT", new ResponseCompletionAdapter(
ack: () =>
{
Console.WriteLine("responecompletionadapter ack");
},
nak: (error) =>
{
Console.WriteLine("responsecompletionadapter [get][location] : danger will robinson!");
},
response: (bytes) =>
{
Console.WriteLine("responsecompletionadapter [get][location] : " + Encoding.UTF8.GetString
(bytes, 0, bytes.Length));
we.Set();
}));
we.WaitOne();

//ensure set before observing
client.doSet("WHAT", "something else", (status) =>
{
Console.WriteLine(status);
we.Set();
});
we.WaitOne();

client.doGet("WHAT", new ResponseCompletionAdapter(
ack: () =>
{
Console.WriteLine("GET ack");
},
nak: (error) =>
{
Console.WriteLine("responsecompletionadapter [get][location] : danger will robinson!");
},
response: (bytes) =>
{
Console.WriteLine("GET RESPONSE [get][location] : " + Encoding.UTF8.GetString(bytes, 0,
bytes.Length));
}));

client.doAction("DOACTION", new ResponseCompletionAdapter(


ack: () =>
{
Console.WriteLine("ACTION ack");
},
nak: (error) =>
{
Console.WriteLine("responsecompletionadapter [action] : danger will robinson!");
},
response: (bytes) =>
{
Console.WriteLine("ACTION RESPONSE [action] : " + Encoding.UTF8.GetString(bytes, 0, bytes.
Length));
}));

client.addStatusListener((status, data) =>
{
Console.WriteLine("{0}:{1}", status, ASCIIEncoding.ASCII.GetString(data));
});

Console.WriteLine("Hello World!");

Console.CancelKeyPress += (o, e) =>
{
we.Set();
};

we.WaitOne();
}
}
}


# Questions/Observations/Enhancements


# Enhancements

List of proposed enhancements to protocol as well as the ECOMMS Class Library and framework.

```
title description proposed
by
```
```
add optional payload to Action it would be useful to tell a participant to do something with this data sne
```
```
add optional payload to Event consider the event experiment started, it would be useful to have the
option to send experiment
```
```
meta data along with event
```
```
update the library to make use of connection statistics and
other information available from the
```
```
server ( broker daemon ) in a general way
```
```
it would be useful to notify clients that the managers connection to the
broker
```
```
has been severed as the fact that all participants will essentially go offline
the additional
```
```
information that we have lost the connection to the broker is very
important
```
```
sne
```
```
provide a way for a participant to report a custom role, type,
and subtype
```
```
currently the role, type, and subtype of a participant is encapsulated in
an enum - to facilitate
```
```
ad-hoc communications provide a way for a participant to report a
custom role, type, and subtype -
```
```
custom will be added to each of the participant properties enums and
clients, when custom is encountered,
```
```
will ask for the custom property
```
```
sne
```
```
ensure that Client subscriptions are removed when
participant goes offline
```
```
Manager creates instances of Client for all discovered Participant's and
listens to them. when a client goes offline
```
```
the Client is removed from the list of Client's maintained by the Manager
but at the momement the subscriptions
```
```
created by the Client are not removed. This will cause the thread count
maintained by the broker client ( NATS.io
```
```
in the alpha reference code) to go up.
```

# Observations


# Questions


# Creating experiments - keeping Client interface instrument

# agnostic and light weight

An extension of:

High level client ( the what ), smart instrument ( the how )

Two ( IMHO ) of the most important and useful things we do with our analytical instruments is creating and running experiments. All of our instruments do
it in one way or another. In ECOMMS, user facing clients expect to see and interact with ALL of the TA ( and with some work ALL ALL ) instruments in the
lab ( on the network, in the building, etc. ). While the act of creating and running an experiment is basically the same across all instruments the experiment
itself will be different based on which instrument will run it. So how can we develop user facing clients ( web apps, desktop apps, etc. ) that can create and
run experiments on any instrument that can be seen ( made available ) by ECOMMS?

Simple. The key to keeping our web and desktop applications light weight with respect to experiments is to 1) keep the instrument specific data structures
( ie. an experiment ) on the instrument ( or in our case in the ECOMMS SHIM ) and 2) devise a neutral protocol that can be used for all instruments by
user facing clients.

So what might that look like?

A lot of ECOMMS is still in design and development but my expectation is that this protocol will look like what follows. It is important to remember that the
instrument and or ECOMMS SHIM will own the experiment and or sequence of experiments and ECOMMS Clients will merely command the instrument
shim as to how to create, modify, and start, the experiment ( and or sequence ).

So as par for the course our application creates a Manager which delivers an InstrumentClient that is a proxy for Dio the DSC. This instrument Client is
our connection to an InstrumentParticipant that is our ECOMMS DSC SHIM.

InstrumentClient InstrumentParticipant ( DSC SHIM )

An instrument Client will send a "create experiment" request to its Participant ( the DSC SHIM in this case ). An instance of an empty, template
experiment will be created ON THE SHIM and a unique identifier, possibly a guid, will be returned by the SHIM. This identifier will then be used by the In
strumentClient to refer to the newly created experiment on the SHIM. Subsequent requests to modify the experiment ( add step ( rheo ), add segment (
thermal ), add meta data like sample name, etc. ) will then include this unique experiment identifer like so, "add step, <some step>, to experiment with
id <id>". This request in turn would return an identifier for the newly created step ( or segment ) which is then used to further configure the step with
something like "update step with <id> set equilibrate temperature to <a temperature>".

Along with a general protocol for the creation and modification of experiments it also makes sense to have a general way of describing an experiment and
its constituent parts ( meta data, steps, and their description ) making it a snap to create UI for these operations. While it is possible to create instrument
specific derived InstrumentClient's we should endeavor to design a general approach to our instrument interactions that keep ( where possible ) instrument
specifics ( data structures and instances ) in the SHIM or instrument.

There is still work to be done to delineate what this general, experiment protocol is but I for one am excited about the possibilities! Keeping the experiment
specifics and instances on the instrument allows Clients to be ephemeral and possible collaboration between Clients.


# Shim Participant development

Or "Instrument Participant" development

What is a shim?

A shim is the software bridge between an instrument or lab equipment to the enterprise communications bus. In our case it is the software that
understands the intricacies of an instruments communications ( think tart for ARES or CII for Mercury ) and how to marshal that communication protocol
onto the enterprise communications bus through gets, sets, actions, and status and the Enterprise Communications over Nats.io client and participant
class libraries.

Shim development is in many ways similar to application development. While the shim code will provide an interface to an instrument through its native
communications and project those comms onto the enterprise communications bus it too may need to know and or interact with, other ECOMMS
participants.

In order to allow our shim code to run on other .NET CORE platforms we need to ensure that the code and DLL's we use are .NET Standard 2.0
compliant. That will be for another discussion.

We have a shim that connects to a Mercury DSC and communicates over ECOMMS. What was involved in getting it onto the bus and start listening for
requests and how does it use the concepts we've been talking about.

ECOMMS shim development will generally start with the creation of a .NET CORE console application. Next we need to determine which DLL's ( plucked
from a Trios build ) are required to interact with the instrument we are providing the shim for. In my case I am developing a shim for a Mercury
DSC. References to the ECOMMS DLL's are also required.

Next we create a class for our shim that inherits from InstrumentParticipant.


```
DSC Instrument SHIM
```
```
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Manager;
using ECOMMS_Participant;
using NATS.Client;
using System;
using System.Linq;
using System.Threading;
using TAInstruments.InstrumentMercuryDSC;
/// <summary>
/// ta enterprise communications
/// sample dsc instrument shim
/// </summary>
namespace ECOMMS_DSC_SHIM
{
class DSCParticipant : InstrumentParticipant
{
DSCInstrument _instrument;
public DSCParticipant(string id, DSCInstrument instrument) : base(id, ECOMMS_Entity.Type.Thermal)
{
_instrument = instrument;
name = _instrument.SerialNumber;
}
}
}
```
Our shim takes a reference to a Mercury DSC instrument and saves to a field.

Next we define an action that our shim will support, Start in this case. We do this by overriding the action method. In our case when the start request is
received we call through the Mercury Instrument interface to perform a start procedure. Also required and expected to be performed by our derived
participant is to reply to the client. This is facilitated by a call to replyTo. In our case ( the start action ) there is nothing to reply except a success or
failure but if there were data to reply ( such as in a get request discussed elsewhere ) it would be passed to replyTo. In more complex interactions
between a client and participant an ack or nak and reply ( response ) will be required and is facilitated by the participant base class with ack() and nak
(error) methods. These concepts and protocol are discussed elsewhere..

```
start action
```
```
public override void action(string what, Msg message)
{
switch(what)
{
case "Start":
ack(message);
if (_instrument.Cortex.StartProcedure())
replyTo(message, "SUCCESS");
else
replyTo(message, "FAILURE");
break;
default:
base.action(what, message);
break;
}
}
```
Along with supporting the start action we want to project Mercury procedure status ( essentially run state ) changes onto ECOMMS. We do this by
overriding the init() method and listening to Cortex property changed events and call through the Participant sendStatus() method.


```
run state
```
```
public override void init()
{
base.init();
_instrument.Cortex.PropertyChanged += (s, a) => {
switch(a.PropertyName)
{
case "ProcedureStatus":
//send status?
//send status changed?
sendStatus("runstate", _instrument.Cortex.ProcedureRunStatus.ToString());
break;
}
};
}
```
OK great we have a shim, WOOHOO! Now how do we start it talking over ECOMMS?

It's easy really. A shim is essentially a console application so we must fill out the Main() method that is provided for us when we create the project. First
we create an instance to our Mercury instrument. In this case we want to talk to Dio the DSC. We are going to write some information about Dio to show
that we are in fact connected to it.

```
Mercury instrument instance
```
```
class Program
{
static void Main(string[] args)
{
Console.WriteLine("DSC (Dio 10.52.54.133) SHIM!");
DSCInstrument instrument = new DSCInstrument();
instrument.Connect("10.52.54.133");
```
```
//show that we are connected to dsc
instrument.PropertyChanged += (s, a) => {
switch(a.PropertyName)
{
case "NodesInitialized":
Console.WriteLine(instrument.Name);
Console.WriteLine(instrument.SerialNumber);
Console.WriteLine(instrument.Cortex.ProcedureRunStatus.ToString());
break;
}
};
}
}
}
```
OK we can talk to Dio our Mercury instrument over its native comms. What's next? In order for our shim to communicate over ECOMMS it needs a unique
id or address. This is what is in its advertisement and allows clients to direct requests at it. To demonstrate how services are participants too I have
created ( described on another page ) an address service who's only job is to deliver the next unique address. In reality any way of creating a unique id
would suffice and could be an integral part of creating a participant but this gives me a way of showing how code can get a reference to a running service
and interact with it.

So what are we talking about here? Integral to the ECOMMS library is this concept of a Manager, an all knowing entity that is listening to the bus so you
don't have to! We simply create an instance and call connect() and init(). connect() will connect to the nats.io server running on localhost. A nats.io url (
"nats://10.52.50.59:5555" ) can be passed in to connect to a server running elsewhere.


```
enterprise manager instance
```
```
Manager enterpriseManager = new Manager();
enterpriseManager.connect();
enterpriseManager.init();
```
The manager instance is our applications/shims view of the enterprise communications bus. The manager can be observed ( the manager and
entities implement the observer pattern discussed in the library pages ) and is used ( initially ) to be informed of participant connections and disconnections
and contains a living list of available participants. Now that we have a manager instance we can get a reference to a client that we can use to ask the addr
ess service for an address that will be used to get our shim participant onto the bus. The manager has a list of clients that is built and maintained by
continuously listening to the bus. In this example I am using linq to get the only service that is available which is my address service. Participants also
have a type which will be used to further differentiate them ( our address service will have a type of Address ). In my contrived example I know there is
only one service running. Let's get the next address.

```
address service
```
```
var services = enterpriseManager.clients.Where((i) => i.role == Role.Service); //just assume one
service for now - will be able to search by name soon
var addressService = services.ElementAt<IClient>(0);
```
With this address service client we can get the next address and use it in our DSCInstrumentParticipant constructor.

```
get next address
```
```
var address = addressService.request("get", "nextaddress"););
```
OK now we're ready to create our shim instance and get it onto the bus and start listening for requests.

```
shim instance
```
```
var dscShim = new DSCParticipant(address, instrument);
dscShim.connect();
dscShim.init();
Console.WriteLine("shim running on " + address);
```
That's all there is to it!

Complete example.

```
Complete example
```
```
using ECOMMS_Client;
using ECOMMS_Entity;
using ECOMMS_Manager;
using ECOMMS_Participant;
using NATS.Client;
using System;
using System.Linq;
```

using System.Threading;
using TAInstruments.InstrumentMercuryDSC;
/// <summary>
/// ta enterprise communications
/// sample dsc instrument shim
/// </summary>
namespace ECOMMS_DSC_SHIM
{
class DSCParticipant : InstrumentParticipant
{
DSCInstrument _instrument;
public DSCParticipant(string id, DSCInstrument instrument) : base(id, ECOMMS_Entity.Type.Thermal)
{
_instrument = instrument;
name = _instrument.SerialNumber;
}

public override void action(string what, Msg message)
{
switch(what)
{
case "Start":
ack(message);
if (_instrument.Cortex.StartProcedure())
replyTo(message, "SUCCESS");
else
replyTo(message, "FAILURE");
break;
default:
base.action(what, message);
break;
}
}

public override void init()
{
base.init();
_instrument.Cortex.PropertyChanged += (s, a) => {
switch(a.PropertyName)
{
case "ProcedureStatus":
//send status?
//send status changed?
sendStatus("runstate", _instrument.Cortex.ProcedureRunStatus.ToString());
break;
}
};
}
}

class Program
{
static void Main(string[] args)
{
Console.WriteLine("DSC (Dio 10.52.54.133) SHIM!");
DSCInstrument instrument = new DSCInstrument();
instrument.Connect("10.52.54.133");

//show that we are connected to dsc
instrument.PropertyChanged += (s, a) => {
switch(a.PropertyName)
{
case "NodesInitialized":
Console.WriteLine(instrument.Name);
Console.WriteLine(instrument.SerialNumber);
Console.WriteLine(instrument.Cortex.ProcedureRunStatus.ToString());
break;
}
};

#region create a manager


Manager enterpriseManager = new Manager();
enterpriseManager.connect();
enterpriseManager.init();
#endregion

//wait for the manager to be initialized
//this will happen inside of manager callback for
//"MANAGER_INITIALIZED"
//but until that happens just wait
Thread.Sleep(5000);

#region get the address service
//show how we can get a reference to a running
//service and ask it questions
var services = enterpriseManager.clients.Where((i) => i.role == Role.Service); //just assume one
service for now - will be able to search by name soon
var addressService = services.ElementAt<IClient>(0);
#endregion

//get an address from the address service
//blocking request to get address from service client
//to be used to create instrument participant and client below
var address = addressService.request("get", "nextaddress");

//create dsc instrument shim instance and get it onto the enterprise bus
var dscShim = new DSCParticipant(address, instrument);
dscShim.connect(); //Rpi nats server
dscShim.init();
Console.WriteLine("shim running on " + address);
Console.ReadKey();
}
}
}


# Instrument Client development


# Service Participant development

The finished solution is here:

svn://10.53.240.28/Trios/users/sne/ECOMMS_SERVICE_DEMO

Follow along as I develop a service participant.

What will this service do?

Log ( write to a text file ) whenever an instrument participant connects/disconnects from the enterprise communications bus and anytime ( Mercury
instrument shims for now ) the run state ( Mercury instrument procedure status ) changes.

Granted this is a contrived example but does showcase how a service is developed and uses the manager and clients to know what is going on on the bus
and has access to all of them.

Ok let's get started.

You will need to build or get the prebuilt C# ECOMMS libraries.

You will also need to build or access to prebuilt AddressService and DSC Instrument SHIM.

And you will need to start a nats.io server running on localhost ( gnatsd.exe -D -V ).

First create a new Visual Studio .NET Core (I have had core in all caps in other places should be Core) console project.

Visit "manage nuget packages" from the project context menu and browse nuget.org for nats.client and install NATS .NET Client.

Add a reference to all four ECOMMS DLL's ( you will need to build these from the svn project mentioned here: Enterprise Communications over Nats.io or
Every Instrument, Every Experiment, Everywhere! ).

Create a class that inherits from ServiceParticipant.


```
using ECOMMS_Entity;
using ECOMMS_Participant;
using System;
namespace ECOMMS_SERVICE_DEMO
{
class DemoService : ServiceParticipant
{
/// <summary>
/// construct a DemoService on an address
/// role is service and type is none for now
/// </summary>
/// <param name="id"></param>
public DemoService(string id) : base(id, Role.Service, ECOMMS_Entity.Type.None)
{
}
}
class Program
{
static void Main(string[] args)
{
Console.WriteLine("Hello World!");
}
}
}
```
And here's the meat of the service. Create a manager instance and connect and init it ( this is what starts it listening ). We are going to observe the enter
prise manager and if it notifies CONNECTED we are going to add an observer to the client that has connected that will do something when RUNSTATE_
CHANGED is notified.


```
public override void init()
{
base.init();
Manager enterpriseManager = new Manager();
enterpriseManager.connect(); //connect to localhost
enterpriseManager.init(); //init
```
```
//addobserver(observerex) notifies with data which is the added client in this case
enterpriseManager.addObserver(new ObserverAdapterEx((o, h, c) =>
{
//need to wait to notify until after base class has gotton response
//to role request
//or have library query first before creating client
//WIP...
Thread.Sleep(1000);
var client = c as IClient;
switch (h)
{
case "CONNECTED":
if (client.role == Role.Instrument)
{
Console.WriteLine(client.name + " INSTRUMENT CONNECTED");
//listen for run state changes
client.addObserver(new ObserverAdapterEx((anobject, hint, data) =>
{
var bytes = (byte[])data;
var anInstrumentClient = (InstrumentClient)anobject;
var say = string.Format("{0} notified {1} with {2}",
client.name,
hint,
Encoding.UTF8.GetString(bytes, 0, bytes.Length)
);
Console.WriteLine(say);
switch (hint)
{
case "RUNSTATE_CHANGED":
break;
}
}));
}
break;
}
}));
}
}
```
Now to get our service onto the bus and start it listening we create an instance and connect and init.

```
class Program
{
static void Main(string[] args)
{
Console.WriteLine("ECOMMS DEMO SERVICE!");
//create our demo service - we're going to generate the address
//instead of asking an address service
DemoService service = new DemoService(Guid.NewGuid().ToString());
service.connect();
service.init();
Console.ReadKey();
}
}
```

Now let's update our callbacks to do something interesting. When an instrument client notifies RUNSTATE_CHANGED lets write a line in a file.

```
switch (hint)
{
case "RUNSTATE_CHANGED":
File.AppendAllLines(@"c:\temp\ecomms_run_state.txt", new List<string>()
{ say });
break;
}
```

# Matrix Calculator, Oh Matrix Calculator...

Wherefore art thou "Matrix Calculator"...

Or "Division of labor between Instrument Clients and Participants"...

The division of labor between an instrument ( what is an instrument anyway? ) and a client application is different for each instrument. One instrument
understands what an experiment is and how to run it while another must be told what to do each step of the way. In the case of Trios this division is
transparent to the user. I fire up Trios and connect to an instrument. It is not apparent to me ( nor do I care ) that the instrument cannot do things for itself
because to me it appears to do what I want.

More coming soon...

(discussion of Instrument Clients and Participants and where stuff should go and can go )


# (WIP) Enterprise application development ( observing the

# manager and clients )

## Overview ( tutorial time: about 30 minutes )

Example Code:

svn://10.53.240.28/Trios/users/sne/ECOMMS_APP

Enterprise application development using the Enterprise Communications framework goes something like this:

```
Create your application ui shell - this is a main form or web page ( in our case a winforms application )
Create an instance of ECOMMS_Manager.Manager
Observe the manager for Participant ( instruments and services are Participants ) connections
Provide a way for users to see what instruments are available ( in our case a listbox )
When the user chooses an instrument provide a view of that instrument ( could include real time signals and runstate, etc. )
```
Get the finished project here:

## Setup

Grab the ECOMMS libraries from here:

Grab the ECOMMS address service demo executable from here:

Grab the ECOMMS shim demo executable from here:

Run the address service and shim demo using the command lines below:

## Development

Fire up Visual Studio and create a new WinForms project ( .NET 4.6.2 ).

Add the NATS client to the project using the Nuget package manager.

Add references to the ECOMMS libraries:

ECOMMS_Client

ECOMMS_Entity

ECOMMS_Manager

Add a listbox to the main form.


Add a form to the project that will be used to observe an instrument and change colors on runstate changes.

Update the new forms constructor to take an IClient

```
public instrumentForm(IClient client)
{
InitializeComponent();
}
```
OK leave the new form, we'll come back to it later.

Create an instance of ECOMMS_Manager.Manager in the Form1 constructor:

```
//create an instance of the enterprise manager
Manager enterpriseManager = new Manager();
enterpriseManager.connect();
enterpriseManager.init();
```
Now observe the Manager to be informed of connections.

When we see a new instrument add an item to the ListBox.


```
//observe the enterprise manager
enterpriseManager.addObserver(new ObserverAdapter((o, h) =>
{
//need to wait to notify until after base class has gotton response
//to role request
//or have library query first before creating client
//WIP...
Thread.Sleep(1000);
switch (h)
{
case "CLIENTS_CHANGED":
var instrumentClients = enterpriseManager.clients.Where((i) => i.role == Role.
Instrument);
_instrumentsListBox.BeginInvoke(new MethodInvoker(() => {
_instrumentsListBox.Items.Clear();
foreach (IClient instrumentClient in instrumentClients)
{
_instrumentsListBox.Items.Add(instrumentClient);
}
}));
break;
}
}));
```
Before we move on let's add a selection handler to the listbox that creates an instance of our new instrument form and passes the selected client to it.

```
//add a selection handler to listbox
_instrumentsListBox.SelectedValueChanged += (s, a) => {
Console.WriteLine("selected");
var client = (s as ListBox).SelectedItem;
var f = new instrumentForm(client as IClient);
f.Show();
};
```
And in the instrumentForm constructor let's get the realtimesignalnames and observe the client for RUNSTATE_CHANGED and datarecord ( these
will be more standardized and constants available in the future, sorry! ). Clients also have a convenient way to listen for status's with addStatusListener(
Action<string, bytes[]> ).

```
public instrumentForm(IClient client)
{
InitializeComponent();
```
```
_instrumentName.Text = client.name;
```
```
_client = client;
```
```
//set the titlebar to the name of the client we are watching
Text = client.name;
```
```
//listen for run state changes
client.addObserver(new ObserverAdapterEx((anobject, hint, data) =>
{
var bytes = (byte[])data;
```
```
var runstate = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
```
```
switch (hint)
{
case "datarecord":
var wireRecord = MercuryWireRecord.Parser.ParseFrom(bytes);
```

Console.WriteLine(wireRecord.Tag);
break;
case "RUNSTATE_CHANGED":
switch(runstate)
{
case "Idle":
BackColor = Color.Green;
break;
default:
BackColor = Color.Red;
break;

}
break;
}

}));

int _datarecords = 0;
client.addStatusListener((what, bytes) =>
{
switch(what)
{
case "realtimesignalsstatus":
var signals = MercuryRealTimeSignalsPB.Parser.ParseFrom(bytes);

if (_signalNamesPB == null) return;

_signalList.BeginInvoke(new MethodInvoker(() =>
{
_signalList.Items.Clear();
for (int i = 0; i < 25; i++)
{
_signalList.Items.Add(String.Format("{0} {1}", _signalNamesPB.Names[i], signals.
Data[i]));
}
}));
break;
case "datarecord":
_datarecordsCount.BeginInvoke(
new MethodInvoker(
() =>
{
_datarecordsCount.Text = String.Format("{0}", _datarecords++);
}
));
break;
}

});

client.doGet("realtimesignalnames", new ResponseCompletionAdapter(
ack: () => { Console.WriteLine("got ack"); },
nak: (error) => { Console.WriteLine("got nak"); },
response: (data) =>
{
Console.WriteLine("got response");
var signalNamesPB = MercuryRealTimeSignalNamesPB.Parser.ParseFrom(data);

_signalNamesPB = signalNamesPB;

foreach (var name in signalNamesPB.Names)
Console.WriteLine(name);
}
));
}


Summary

No summary yet!


# ECOMMS Framework - Interface


# ECOMMS Framework - Participant


# ECOMMS Framework - Client


# Packages


# How to run demo DSC shim

Update 12/07/2018

As below an ip address can be passed to the ECOMMS_DSC_SHIM. This determines which instrument the shim is "projecting" onto
ECOMMS. What I forgot to mention is that a nats url can also be passed on the command line to control which nats server is used like so:

dotnet ECOMMS_DSC_SHIM.dll 10.52.54.133 nats://aberdeen1:4222

In order for the shim to work properly it need to find an "address" service running. This was done as an example of how shims and services (
both participants ) can interact. Consider that there will be a number of services running ( persistent store, variables manager, matrix
calculator, etc. ) and this example shows how they interact. The address service ( TestService project in the ECOMMS_DSC_SHIM solution )
has been updated to take a nats url on the command line as well:

dotnet TestService.dll nats://aberdeen1:422

So that our shim and address service can be started on the same nats server.

Update 11/27/2018

The code was updated in October to take an ip address on the command line allowing multiple instance to be fired up and connected to
different DSC's. The build artifacts have been checked in and all that is required to run the shim is to check out from here:

```
svn://10.53.240.28/Trios/users/sne/ECOMMS_DSC_SHIM
```
and follow the instructions below and pass the ip address on the command line when starting the shim.

dotnet ECOMMS_DSC_SHIM.dll 10.52.54.133

Bear with me as this will get easier

For the early EARLY adopters here is the nitty gritty to getting the bleeding edge TA Enterprise Communications SHIM example running...

The concepts below are ( will be in some cases ) explained in greater detail in earlier pages.

What this demo does:

Runs an address service. This is a demo service that responds with a unique identifier used to get our SHIM onto the bus.

Runs a Mercury Thermal DSC SHIM that connects to a DSC ( Dio to be specific ) and can perform a request to start an experiment.

The SHIM code does this:

```
creates an instances of Manager
asks the Manager for all of the available services ( there will be only one for this demo )
assumes an address service and asks it for the next address
creates an instance of our demo DSC Instrument Participant ( SHIM ) on the returned address
and gets it onto the bus
```
Runs a console app that:

```
creates an instances of Manager
asks the Manager for all of the available instruments ( there will be only one for this demo )
uses the returned client to send the start action to the DSC instrument
```
OK Let's go!\ECOMMS_DSC_SHIM\TestService\bin\Debug\netcoreapp2.1

First make sure you have downloaded the NATS.IO server and that it is running ( the command line is below ):


```
gnatsd.exe -D -V
```
This will run the server on localhost, port 4222

Also make sure you have installed .Net Core.

Now checkout the DSC SHIM demo from here ( all of the demo has been built ):

```
svn://10.53.240.28/Trios/users/sne/ECOMMS_DSC_SHIM
```
Start up Trios and connect to Dio @10.52.54.133 ( Mercury Thermal DSC ) and create a simple experiment in the run queue.

Now run the address service:

\ECOMMS_DSC_SHIM\TestService\bin\Debug\netcoreapp2.1>dotnet TestService.dll

Run the SHIM:

\ECOMMS_DSC_SHIM\ECOMMS_DSC_SHIM\bin\Debug\netcoreapp2.1>dotnet ECOMMS_DSC_SHIM.dll

Run the test client:

\ECOMMS_DSC_SHIM\ConsoleApp1\bin\Debug\netcoreapp2.1>dotnet TestClient.dll

Upon running the test client the experiment will start...

**steve**


# ECOMMS_VUE - Follow Along!

Follow along as I build an ecomms web page completely in javascript ( in my ME time ).

Provide access to ecomms resources from a web site using javascript frameworks.


# possible with ecomms

what is possible when the actors in the system are addressable, discoverable, identifiable, and are all on the same universal bus? what is possible with this
approach to our systems and systems communications?


# introduction

ereh seog gnihtemos...


# address and role

simple concepts that are extremely powerful but you'd be surprised how many miss this simple premise. these concepts are often conflated.

back at hologic i was designing the client side of the gantry (where the xray tube, detector, and mechanicals resided) communications but worked closely
with the gantry team to hammer out the communications protocol. in that system the can bus was extended out of the gantry to the client
workstation. actors on the can bus needed to be identified (who), addressable (where), conversed with ( requests ), and listened to ( status ). the
gantry actors were assigned an identifier that was fixed. the detector was one, the xray was two, and so on. most of the actors were required for
exposure and readiness was reported in an actors heartbeat. i quickly piped up and suggested that actor role should be separate from actor address. t
hat the role an actor plays in the system ( the gantry in this case ) is not it's address and vice-versa. address is where to direct requests and role
provided the list of supported requests. of course being the client side guy my "whining" fell on deaf ears. the reasoning goes "this is a proprietary
system and we have wakawaka control over all of the parts so there is no need for this". of course, until they began design of a new system that
incorporated two detectors.

so what does address and role buy us?

in ecomms address and role in large part is what makes system actors discoverable (what instruments are available), addressable (which instrument am
i conversing with), and identifiable ( based on role and type what is this instruments interface ). this same concept can ( and should ) be applied to
accessories and instrument modules ( nodes ) inside and around the instrument. certainly, today, an instrument is made of ( at least some thermal
instruments ) an autosampler, gdm ( gas delivery module ), fep ( front-end power), fcm ( front-end control module ). these actors are inside the box but
imagine if we could bring them together at run time? imagine that an instrument, a system, is what is available at the time an experiment is requested. wh
at if we could direct an autosampler to deliver a sample to any number of available instruments or have two gdm's in a system? this is where address and
role comes in. creating systems with many actors that perform the same role but keeping them discoverable and identifiable is what address and role
provide.



