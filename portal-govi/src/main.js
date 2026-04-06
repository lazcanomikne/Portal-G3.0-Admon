import axios from "axios";
import Vue from "vue";
import excel from "vue-excel-export";
import App from "./App.vue";
import vuetify from "./plugins/vuetify";

import router from "./router";
import store from "./store";

// Eagerly clear session if we are on the login page to prevent unauthorized redirects on refresh
// We do this at the very top of the entry point to ensure it runs before any other initialization
if (window.location.hash.includes("login")) {
  localStorage.removeItem("jwt");
  localStorage.removeItem("b1session");
  localStorage.removeItem("routeid");
  localStorage.removeItem("govi_session");
}

const host = window.location.hostname;

//let url = "";
//url = "http://192.168.1.206:8087";
//url = `http://${host}:8081`;
//url = 'https://localhost:5001'

let url = process.env.VUE_APP_BASE_URL || "/api";
//url = 'https://localhost:44393'
//url = "http://172.28.124.54:8080";

Vue.use(excel);
Vue.config.productionTip = false;

Vue.filter("currency", (value) => {
  let val = (value / 1).toFixed(2).replace(",", ".");
  return "$" + val.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
});
Vue.filter("textcrop", (value, len) => {
  return value.length > len ? value.substr(0, len) + "..." : value;
});
Vue.filter("textcrop2", (value, len) => {
  return value.length > len ? value.substr(0, len) : value;
});

const axiosInstance = axios.create({
  baseURL: url,
  params: {},
  headers: {
    Accept: "application/json",
    "Content-Type": "application/json; charset=utf8",
  },
});

axiosInstance.interceptors.request.use(function (config) {
  // If the request starts with /api/, remove it because baseURL already includes it
  // and we removed "api/" from the backend controllers.
  if (config.url && config.url.startsWith("/api/")) {
    config.url = config.url.replace("/api/", "/");
  } else if (config.url && config.url.startsWith("api/")) {
    config.url = config.url.replace("api/", "/");
  }

  const routeid = localStorage.getItem("routeid");
  const b1session = localStorage.getItem("b1session");
  config.headers["routeid"] = routeid;
  config.headers["b1session"] = b1session;
  return config;
});

Vue.prototype.$axios = axios;

export { axiosInstance };

new Vue({
  router,
  store,
  vuetify,
  render: (h) => h(App),
}).$mount("#app");
