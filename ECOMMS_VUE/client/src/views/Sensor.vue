<template>
  <div class="sensor">
    <img alt="Vue logo" src="../assets/logo.png">
    <SensorTest />
  </div>
</template>

<script>
// @ is an alias to /src
import SensorTest from '@/components/SensorTest.vue'

export default {
  name: 'sensor',
  components: {
    SensorTest
  }
}
</script>
