<script setup>
import { ref } from "vue";
const isExpanded = ref(false);
const route = useRoute();

function toggle() {
  return true;
}
</script>
<template>
  <header>
    <nav
      class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3"
    >
      <div class="container">
        <NuxtLink class="navbar-brand" to="/">CleanArchitecture.WebUI</NuxtLink>
        <button
          class="navbar-toggler"
          type="button"
          data-toggle="collapse"
          data-target=".navbar-collapse"
          aria-label="Toggle navigation"
          :aria-expanded="isExpanded"
          :click="toggle()"
        >
          <span class="navbar-toggler-icon"></span>
        </button>
        <div
          class="navbar-collapse collapse d-sm-inline-flex justify-content-end"
          :class="{ show: isExpanded }"
        >
          <ul class="navbar-nav flex-grow">
            <li id="index" class="nav-item">
              <NuxtLink class="nav-link text-dark" to="/" exact>Home</NuxtLink>
            </li>
            <li id="counter" class="nav-item">
              <NuxtLink class="nav-link text-dark" to="/counter" exact
                >Counter</NuxtLink
              >
            </li>
            <li id="fetch-data" class="nav-item">
              <NuxtLink class="nav-link text-dark" to="/fetchdata" exact
                >Fetch data</NuxtLink
              >
            </li>
            <li id="todo" class="nav-item">
              <NuxtLink class="nav-link text-dark" to="/todo" exact
                >Todo</NuxtLink
              >
            </li>
            <li id="token" class="nav-item">
              <NuxtLink class="nav-link text-dark" to="/token" exact
                >JWT</NuxtLink
              >
            </li>
            <li class="nav-item">
              <a class="nav-link text-dark" href="/api" target="_blank">API</a>
            </li>
          </ul>
          <LoginMenu></LoginMenu>
        </div>
      </div>
    </nav>
  </header>
</template>
<style>
a.nuxt-link-exact-active {
  color: #00c58e;
}
</style>