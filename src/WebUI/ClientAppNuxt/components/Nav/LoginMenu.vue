<script>
const isAuthenticated = ref(false);
const userName = ref("test user name");
</script>
<template>
  <ul class="navbar-nav" v-if="isAuthenticated">
    <li class="nav-item">
      <NuxtLink
        class="nav-link text-dark"
        [routerLink]='["/NuxtLinkuthentication/profile"]'
        title="Manage"
        >Hello {{ userName }}</NuxtLink
      >
    </li>
    <li class="nav-item">
      <NuxtLink
        class="nav-link text-dark"
        [routerLink]='["/NuxtLinkuthentication/logout"]'
        [state]="{ local: true }"
        title="Logout"
        >Logout</NuxtLink
      >
    </li>
  </ul>
  <ul class="navbar-nav" v-if="!isAuthenticated">
    <li class="nav-item">
      <NuxtLink
        class="nav-link text-dark"
        [routerLink]='["/NuxtLinkuthentication/register"]'
        >Register</NuxtLink
      >
    </li>
    <li class="nav-item">
      <NuxtLink
        class="nav-link text-dark"
        [routerLink]='["/NuxtLinkuthentication/login"]'
        >Login</NuxtLink
      >
    </li>
  </ul>
</template>