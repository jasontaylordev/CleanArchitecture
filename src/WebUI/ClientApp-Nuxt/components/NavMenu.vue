<template>
  <header>
    <BNavbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
      <BNavbarBrand tag={Link} to="/">WebUI</BNavbarBrand>
      <BNavbarToggle onClick={this.toggleNavbar} className="mr-2" />
      <BCollapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
        <ul className="navbar-nav flex-grow">
          <BNavItem>
            <b-link tag={Link} href="/" target="_blank" class="text-dark">Home</b-link>
          </BNavItem>
          <BNavItem>
            <b-link tag={Link} href="/counter" className="text-dark">Counter</b-link>
          </BNavItem>
          <BNavItem>
            <b-link tag={Link} href="/fetch-data" className="text-dark">Fetch data</b-link>
          </BNavItem>
          <BNavItem>
            <a class="nav-link text-dark" href="/Identity/Account/Manage">Account</a>
          </BNavItem>
        </ul>
      </BCollapse>
    </BNavbar>
  </header>
</template>
<script setup>
const toggle = ref(false);

function toggleNavbar () {
  toggle.value = !toggle.value;
}

</script>
<style>
a.navbar-brand {
  white-space: normal;
  text-align: center;
  word-break: break-all;
}

html {
  font-size: 14px;
}
@media (min-width: 768px) {
  html {
    font-size: 16px;
  }
}

.box-shadow {
  box-shadow: 0 .25rem .75rem rgba(0, 0, 0, .05);
}

</style>