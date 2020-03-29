<template>
  <div class="sensor-list">
    <h1>Sensor test</h1>

    <ul>
      <li :key=sensor.id v-for="sensor in sensors">
        {{ sensor.name }}@{{sensor.location}} | {{sensor.status}}
        <v-btn color="success" @click="onClickHigh(sensor.id)">high</v-btn>
        <v-btn color="success" @click="onClickLow(sensor.id)">low</v-btn>
      </li>
    </ul>

  </div>
</template>

<script>
export default {
  data:()=>({
    status: "a status",
    sensors: []
  }),
  name: 'SensorTest',
  props: {
  },
  mounted(){
    //get the list of services

    console.log('mounted');

    this.$services.sensors.find({}).then(results =>{
      this.sensors = results.data;
    })
  },
  methods: {
    onClickHigh(id){
      console.log("on click:" + id);
      this.$services.sensors.get(
        id,
        {
          query:{
            action: 'get',
            data: 'high'
          }
        }).then(
          answer => {
            console.log(answer);
            alert(answer);
          }
        );
    },

    onClickLow(id){
      console.log("on click:" + id);
      this.$services.sensors.get(
        id,
        {
          query:{
            action: 'get',
            data: 'low'
          }
        }).then(
          answer => {
            console.log(answer);
            alert(answer);
          }
        );
    }
  },

  feathers: {
    sensors:{
      created(sensor){
        this.sensors.push(sensor);
      },

      updated(sensor){
        this.$services.sensors.find({}).then(results =>{
          this.sensors = results.data;
        });
      },

      removed(sensor){
        this.$services.sensors.find({}).then(results =>{
          this.sensors = results.data;
        });
      }
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
