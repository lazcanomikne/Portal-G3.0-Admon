<template>
  <v-app id="inspire">
    <v-navigation-drawer 
      v-model="drawer" 
      :mini-variant="mini" 
      :permanent="$vuetify.breakpoint.mdAndUp"
      app 
      fixed
      clipped
    >
      <!-- Profile Header -->
      <v-list class="py-2">
        <v-list-item class="px-2">
          <v-list-item-avatar color="primary">
            <v-img :src="`https://ui-avatars.com/api/?name=${userName}&background=f8a102&color=152332&bold=true`"></v-img>
          </v-list-item-avatar>

          <v-list-item-title class="title text-h6 font-weight-bold">
            {{ userName }}
          </v-list-item-title>
        </v-list-item>
      </v-list>

      <v-divider></v-divider>

      <!-- Menu Items -->
      <v-list dense nav class="mt-2">
        <v-list-item to="/" rounded="pill" class="mb-1">
          <v-list-item-icon>
            <v-icon>home</v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>Inicio</v-list-item-title>
          </v-list-item-content>
        </v-list-item>

        <template v-for="menu in doneMenu">
          <v-list-group
            :key="menu.Id || menu.id"
            no-action
            v-if="(menu.SubMenu || menu.subMenu || menu.SubMenus || menu.subMenus) && (menu.SubMenu || menu.subMenu || menu.SubMenus || menu.subMenus).length"
            :prepend-icon="(menu.Icon || menu.icon) === 'config' ? 'settings' : (menu.Icon || menu.icon || 'folder')"
          >
            <template v-slot:activator>
              <v-list-item-content>
                <v-list-item-title>{{ menu.Tag || menu.tag }}</v-list-item-title>
              </v-list-item-content>
            </template>

            <v-list-item
              v-for="sub in (menu.SubMenu || menu.subMenu || menu.SubMenus || menu.subMenus)"
              :key="sub.Id || sub.id"
              :to="sub.Path || sub.path"
              @click="navegar(sub)"
              rounded="pill"
              class="mb-1"
            >
              <v-list-item-content>
                <v-list-item-title>{{ sub.Tag || sub.tag }}</v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list-group>
        </template>

      </v-list>

      <template v-slot:append>
        <div class="pa-2 text-center caption text--secondary" v-if="!mini">
           @mm v5.0 | {{ new Date().getFullYear() }}
        </div>
      </template>
    </v-navigation-drawer>

    <v-app-bar app fixed id="appbar" class="glass-header px-4" flat clipped-left>
      <div class="d-flex align-center brand-group">
        <!-- Brand Logo -->
        <v-img
          :src="logoSrc"
          max-height="40"
          max-width="120"
          contain
          class="logo-brand"
        ></v-img>

        <v-toolbar-title class="font-weight-black brand-title mb-0 mx-4">Portal GOVI</v-toolbar-title>
        
        <v-app-bar-nav-icon @click="toggleDrawer"></v-app-bar-nav-icon>
      </div>

      <v-spacer></v-spacer>

      <v-tooltip bottom>
        <template v-slot:activator="{ on, attrs }">
          <v-btn @click="darkMode" icon v-bind="attrs" v-on="on">
            <v-icon>{{
              $vuetify.theme.isDark ? "light_mode" : "dark_mode"
            }}</v-icon>
          </v-btn>
        </template>
        <span>{{ $vuetify.theme.isDark ? "Claro" : "Oscuro" }}</span>
      </v-tooltip>
      <v-btn @click="salir" icon>
        <v-icon>logout</v-icon>
      </v-btn>
    </v-app-bar>

    <v-main>
      <v-fade-transition appear>
        <router-view />
      </v-fade-transition>
    </v-main>
  </v-app>
</template>

<script>
import { mapActions, mapGetters, mapState } from "vuex";

export default {
  name: "MainLayout",
  data: () => ({
    selectedItem: 1,
    drawer: true,
    mini: false,
  }),
  methods: {
    ...mapActions("config", ["getMenu"]),
    toggleDrawer() {
      if (this.$vuetify.breakpoint.mdAndUp) {
        this.mini = !this.mini;
      } else {
        this.drawer = !this.drawer;
      }
    },
    salir () {
      localStorage.removeItem("jwt");
      this.$store.commit("config/SET_CANCREATE", null);
      this.$store.commit("login/SET_USERNAME", '');
      this.$store.commit("login/SET_USERPASS", '');
      this.$router.push({ name: "login" });
    },
    darkMode () {
      this.$vuetify.theme.isDark = !this.$vuetify.theme.isDark;
      this.dark = this.$vuetify.theme.isDark;
      localStorage.setItem("dark", this.$vuetify.theme.isDark ? true : false);
    },
    navegar (item) {
      this.$store.commit("config/SET_CANCREATE", item.CanCreate);
      this.$store.commit("informes/SET_TITLE", item.Tag);
      this.$router.push({ path: item.Path, params: item.CanCreate });
    },
  },
  computed: {
    ...mapGetters("config", ["doneMenu"]),
    ...mapState("login", ["userName"]),
    logoSrc() {
      // User requested: positive for dark, negative for light
      return this.$vuetify.theme.isDark 
        ? require("@/assets/logo/logo_positive.png") 
        : require("@/assets/logo/logo_negative.png");
    }
  },
  mounted () {
    this.getMenu();
  },
};
</script>

<style scoped>
/* No scoped styles needed, global styles handle the theme */
</style>
