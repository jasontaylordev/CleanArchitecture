<script>
  const counter = ref(0);
</script>
<template>
  <div>
    <h1>Counter</h1>

    <p>This is a simple example of a Nuxt component.</p>

    <p aria-live="polite">Current count: <strong>{{ counter }}</strong></p>

    <button className="btn btn-primary" @click="counter++">Increment</button>
  </div>
</template>
