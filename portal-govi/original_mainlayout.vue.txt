<template>
  <v-app id="inspire">
    <v-navigation-drawer
      v-model="drawer"
      fixed
      app
    >
      <v-list dense>
        <v-list-item>
          <v-list-item-content>
            <v-list-item-title class="title">
              Menu
            </v-list-item-title>
            <v-list-item-subtitle>
              Opciones
            </v-list-item-subtitle>
          </v-list-item-content>
        </v-list-item>
      </v-list>
      <v-divider></v-divider>
      <v-list dense>
        <v-list-item to="/">
          <v-list-item-icon>
            <v-icon>
              home
            </v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>
              Inicio
            </v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
      <v-list
        dense
        expand
        nav
      >
        <v-list-group no-action>
          <template #activator>
            <v-list-item-icon>
              <v-icon>
                account_balance
              </v-icon>
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title> Bancos </v-list-item-title>
            </v-list-item-content>
          </template>
          <v-list-item-group v-model="selectedItem">
            <v-list-item to="/dispersion">
              <v-list-item-icon>
                <v-icon>
                </v-icon>
              </v-list-item-icon>
              <v-list-item-content>
                <v-list-item-title>
                  Dispersion
                </v-list-item-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item to="/dispersion_1">
              <v-list-item-icon>
                <v-icon>
                </v-icon>
              </v-list-item-icon>
              <v-list-item-content>
                <v-list-item-title>
                  Dispersion 1 a 1
                </v-list-item-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item to="/informes">
              <v-list-item-icon>
                <v-icon>
                </v-icon>
              </v-list-item-icon>
              <v-list-item-content>
                <v-list-item-title>
                  Informe de bancos
                </v-list-item-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item to="/generados">
              <v-list-item-icon>
                <v-icon>
                </v-icon>
              </v-list-item-icon>
              <v-list-item-content>
                <v-list-item-title>
                  Folios generados
                </v-list-item-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item to="/tunel">
              <v-list-item-icon>
                <v-icon>
                </v-icon>
              </v-list-item-icon>
              <v-list-item-content>
                <v-list-item-title>
                  Tunel bancario
                </v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list-item-group>
        </v-list-group>
      </v-list>
      <v-list
        dense
        expand
        nav
      >
        <v-list-group no-action>
          <template #activator>
            <v-list-item-icon>
              <v-icon>
                account_balance
              </v-icon>
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title> Inventario </v-list-item-title>
            </v-list-item-content>
          </template>
          <v-list-item-group v-model="selectedItem">
            <v-list-item to="/ajusteentrada">
              <v-list-item-icon>
                <v-icon>
                </v-icon>
              </v-list-item-icon>
              <v-list-item-content>
                <v-list-item-title>
                  Ajuste de entrada
                </v-list-item-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item to="/ajustesalida">
              <v-list-item-icon>
                <v-icon>
                </v-icon>
              </v-list-item-icon>
              <v-list-item-content>
                <v-list-item-title>
                  Ajuste de salida
                </v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list-item-group>
        </v-list-group>
      </v-list>
      <v-footer absolute>
        <v-col
          class="text-center"
          cols="12"
        >
          <strong>Leo Lazcano | @ilbeV3.0 | </strong>{{ new Date().getFullYear() }}
        </v-col>
      </v-footer>
    </v-navigation-drawer>

    <v-app-bar
      app
      fixed
      id="appbar"
    >
      <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>

      <v-toolbar-title>Portal GOVI</v-toolbar-title>
      <v-spacer></v-spacer>

      <v-btn
        @click="salir"
        icon
      >
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

export default {
  name: 'MainLayout',
  data: () => ({
    selectedItem: 1,
    drawer: null
  }),
  methods: {
    salir () {
      localStorage.removeItem('jwt')
      this.$router.push({ path: '/login' })
    }
  }
};
</script>
