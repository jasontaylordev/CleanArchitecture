<template>
<table v-if="!pending" className="table table-striped" aria-labelledby="tableLabel">
        <thead>
          <tr>
            <th>Date</th>
            <th>Temp. (C)</th>
            <th>Temp. (F)</th>
            <th>Summary</th>
          </tr>
        </thead>
        <tbody>          
            <tr v-for="forecast in forecasts" v-key="forecast.date">
              <td>{{new Date(forecast.date).toLocaleDateString()}}</td>
              <td>{{forecast.temperatureC}}</td>
              <td>{{forecast.temperatureF}}</td>
              <td>{{forecast.summary}}</td>
            </tr>
        </tbody>
      </table>
</template>
<script setup>
    const forecasts = ref([]);
    const client = new WeatherForecastClient();
    const {data, error, pending} = useAsyncData("forecasts", () => {
        client.get();
    });

    if(error.value) {
        console.log(error.value);
    }

    if (data.value) {
        forecasts.value = data.value;
    }

</script>