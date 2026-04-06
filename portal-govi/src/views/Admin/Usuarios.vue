<template>
  <v-container fluid class="pa-6 usuarios-dashboard">
    <!-- Top Action Bar -->
    <v-toolbar dense flat class="glass-toolbar mb-6 rounded-xl elevation-3 border-thin">
      <v-icon color="primary" class="mr-3">shield_person</v-icon>
      <v-toolbar-title class="font-weight-black brand-secondary--text">
        Usuarios <span class="font-weight-light grey--text">y Permisos</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      
      <v-btn class="glass-btn-icon" icon @click="getUsuarios" :loading="loading">
        <v-icon>refresh</v-icon>
      </v-btn>
    </v-toolbar>

    <v-row>
      <!-- Left Column: KPIs and Table -->
      <v-col cols="12" lg="8">
        <!-- Compact Metrics -->
        <v-row dense class="mb-4">
          <v-col cols="12" sm="6">
            <v-card class="metric-card-compact glass-card rounded-xl pa-3 border-left-primary shadow-premium">
              <div class="d-flex align-center justify-space-between">
                <div>
                  <div class="text-overline grey--text line-height-tight mb-0">Total Usuarios</div>
                  <div class="text-h5 font-weight-black primary--text">{{ usuarios.length }}</div>
                </div>
                <v-avatar color="primary lighten-4" size="40">
                  <v-icon color="primary" size="24">people</v-icon>
                </v-avatar>
              </div>
            </v-card>
          </v-col>
          <v-col cols="12" sm="6">
            <v-card class="metric-card-compact glass-card rounded-xl pa-3 border-left-success shadow-premium">
              <div class="d-flex align-center justify-space-between">
                <div>
                  <div class="text-overline grey--text line-height-tight mb-0">Sincronizados SAP</div>
                  <div class="text-h5 font-weight-black success--text">{{ usuarios.length }}</div>
                </div>
                <v-avatar color="success lighten-4" size="40">
                  <v-icon color="success" size="24">sync</v-icon>
                </v-avatar>
              </div>
            </v-card>
          </v-col>
        </v-row>

        <!-- Main Data Table -->
        <v-card class="glass-card rounded-xl overflow-hidden shadow-premium border-thin">
          <v-data-table
            :headers="headers"
            :items="filteredItems"
            :loading="loading"
            class="glass-table premium-table"
            dense
            fixed-header
            height="calc(100vh - 320px)"
            :items-per-page="50"
            :footer-props="{ 'items-per-page-options': [50, 100, -1] }"
          >
            <!-- Custom Headers for Filtering -->
            <template v-for="h in headers" v-slot:[`header.${h.value}`]="{ header }">
              <div :key="h.value" class="d-flex flex-column py-2">
                <span class="mb-1">{{ header.text }}</span>
                <v-text-field
                  v-if="!['acciones'].includes(h.value)"
                  v-model="filters[h.value]"
                  dense
                  hide-details
                  filled
                  flat
                  solo
                  background-color="rgba(0,0,0,0.03)"
                  class="compact-filter-input"
                  placeholder="Buscar..."
                  clearable
                ></v-text-field>
              </div>
            </template>

            <template v-slot:item.UserName="{ item }">
              <div class="d-flex align-center">
                <v-avatar size="24" color="primary lighten-4" class="mr-2">
                  <v-icon small color="primary">person</v-icon>
                </v-avatar>
                <span class="font-weight-bold">{{ item.UserName || item.userName }}</span>
              </div>
            </template>

            <template v-slot:item.Password="{ item }">
              <div class="d-flex align-center">
                <span class="grey--text caption mr-2" style="min-width: 80px">
                  {{ showPasswords[item.UserName || item.userName] ? (item.Password || item.password) : '********' }}
                </span>
                <v-btn icon x-small @click="$set(showPasswords, item.UserName || item.userName, !showPasswords[item.UserName || item.userName])">
                  <v-icon x-small>{{ showPasswords[item.UserName || item.userName] ? 'visibility_off' : 'visibility' }}</v-icon>
                </v-btn>
              </div>
            </template>
            
            <template v-slot:item.acciones="{ item }">
              <div class="d-flex justify-end">
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn icon small color="primary" v-bind="attrs" v-on="on" @click="abrirPermisos(item)">
                      <v-icon small>lock_open</v-icon>
                    </v-btn>
                  </template>
                  <span>Gestionar Permisos</span>
                </v-tooltip>
              </div>
            </template>

            <template v-slot:no-data>
              <div class="text-center pa-10">
                <v-icon size="64" color="grey lighten-2">face</v-icon>
                <p class="text-h6 grey--text text--lighten-1 mt-4">No se encontraron usuarios</p>
              </div>
            </template>
          </v-data-table>
        </v-card>
      </v-col>

      <!-- Right Column: Management Panel -->
      <v-col cols="12" lg="4">
        <v-card class="glass-card rounded-xl pb-6 shadow-premium border-thin sticky-panel">
          <v-toolbar flat dense class="glass-toolbar-inner rounded-t-xl mb-4">
            <v-icon color="primary" class="mr-2">person_add</v-icon>
            <span class="subtitle-1 font-weight-black brand-secondary--text">
              Nuevo Usuario
            </span>
          </v-toolbar>

          <v-card-text class="px-6">
            <v-alert type="info" text dense icon="info" class="rounded-lg mb-6 text-caption">
              El password debe coincidir con el de SAP para permitir la sincronización.
            </v-alert>

            <v-form ref="formUser" v-model="validUser">
              <v-text-field 
                v-model="newUser.UserName" 
                label="Usuario (UserName) *" 
                filled dense rounded
                prepend-inner-icon="person"
                class="mb-2"
                :rules="[v => !!v || 'Requerido']"
              ></v-text-field>

              <v-text-field 
                v-model="newUser.Password" 
                label="Contraseña *" 
                :type="showNewPass ? 'text' : 'password'"
                filled dense rounded
                prepend-inner-icon="key"
                :append-icon="showNewPass ? 'visibility_off' : 'visibility'"
                @click:append="showNewPass = !showNewPass"
                class="mb-4"
                :rules="[v => !!v || 'Requerido']"
              ></v-text-field>
            </v-form>

            <div class="d-flex justify-center mb-6">
              <v-btn 
                color="teal" 
                text 
                small
                rounded
                :loading="testingSap" 
                @click="probarSap"
                :disabled="!newUser.UserName || !newUser.Password"
              >
                <v-icon left small>sync_alt</v-icon> Test Login SAP
              </v-btn>
            </div>
          </v-card-text>

          <v-card-actions class="px-6">
            <v-btn 
              color="primary" 
              block 
              @click="guardarUsuario" 
              :disabled="!validUser" 
              :loading="loading" 
              class="brand-btn py-6 shadow-premium"
            >
              <v-icon left>person_add</v-icon>
              Crear Usuario
            </v-btn>
          </v-card-actions>
        </v-card>

        <!-- Help Card -->
        <v-card class="glass-card rounded-xl mt-4 pa-4 border-thin shadow-sm bg-faint">
          <div class="d-flex align-center mb-2">
            <v-icon small color="primary" class="mr-2">help_outline</v-icon>
            <span class="text-caption font-weight-bold grey--text text-uppercase">Ayuda</span>
          </div>
          <p class="text-caption grey--text mb-0">
            Al crear un usuario, este tendrá acceso predeterminado nulo. Use el botón de <strong>llave</strong> en la tabla para asignar permisos a los módulos.
          </p>
        </v-card>
      </v-col>
    </v-row>

    <!-- Permissions Dialog -->
    <v-dialog v-model="dialogPermisos" max-width="800px" scrollable transition="dialog-bottom-transition">
      <v-card class="glass-card rounded-xl overflow-hidden">
        <v-toolbar flat dense class="glass-toolbar-inner">
          <v-icon color="primary" class="mr-2">vpn_key</v-icon>
          <span class="text-h6 font-weight-black brand-secondary--text">
            Permisos: <span class="primary--text">{{ selectedUser }}</span>
          </span>
          <v-spacer></v-spacer>
          <v-btn icon small @click="dialogPermisos = false"><v-icon>close</v-icon></v-btn>
        </v-toolbar>

        <v-divider></v-divider>

        <v-card-text class="pa-6" style="height: 500px;">
          <v-expansion-panels flat accordion class="transparent-panels">
            <v-expansion-panel 
              v-for="menu in menuTree" 
              :key="menu.Id || menu.id"
              class="glass-card-panel mb-3 rounded-xl border-thin overflow-hidden"
            >
              <v-expansion-panel-header class="font-weight-bold py-4">
                <template v-slot:default="{ open }">
                  <div class="d-flex align-center w-100">
                    <v-checkbox 
                      v-model="menu.hasAccess" 
                      color="primary" 
                      hide-details 
                      class="mr-3 mt-0 pt-0"
                      @click.native.stop
                      @change="toggleMenuChildren(menu)"
                    ></v-checkbox>
                    <v-avatar size="32" :color="open ? 'primary' : 'grey lighten-3'" class="mr-3 transition-swing">
                      <v-icon small :color="open ? 'white' : 'grey darken-1'">{{ menu.Icon || menu.icon || 'folder' }}</v-icon>
                    </v-avatar>
                    <span :class="open ? 'primary--text' : ''">{{ menu.Tag || menu.tag }}</span>
                  </div>
                </template>
              </v-expansion-panel-header>
              <v-expansion-panel-content>
                <v-list dense class="transparent pt-0">
                  <v-list-item 
                    v-for="sub in (menu.SubMenus || menu.subMenus || menu.SubMenu || menu.subMenu)" 
                    :key="sub.Id || sub.id" 
                    class="rounded-lg mb-1 submenu-item"
                    :class="(sub.hasAccess || sub.HasAccess) ? 'bg-selected' : ''"
                  >
                    <v-list-item-action class="mr-4">
                      <v-checkbox v-model="sub.hasAccess" color="primary" hide-details @change="updateChildPermission(menu, sub)"></v-checkbox>
                    </v-list-item-action>
                    
                    <v-list-item-content>
                      <v-list-item-title class="font-weight-bold">{{ sub.Tag || sub.tag }}</v-list-item-title>
                      <v-list-item-subtitle class="caption grey--text">{{ sub.path }}</v-list-item-subtitle>
                    </v-list-item-content>

                    <v-list-item-action class="flex-row align-center">
                      <v-chip
                        v-if="sub.hasAccess"
                        small
                        :color="sub.canCreate ? 'success' : 'grey lighten-2'"
                        :text-color="sub.canCreate ? 'white' : 'grey darken-3'"
                        class="mr-4 font-weight-bold"
                        label
                      >
                        {{ sub.canCreate ? 'EDICIÓN' : 'LECTURA' }}
                      </v-chip>
                      
                      <v-switch 
                        v-model="sub.canCreate" 
                        dense 
                        hide-details
                        :disabled="!sub.hasAccess"
                        color="success"
                        inset
                        class="mt-0"
                      ></v-switch>
                    </v-list-item-action>
                  </v-list-item>
                </v-list>
              </v-expansion-panel-content>
            </v-expansion-panel>
          </v-expansion-panels>
        </v-card-text>

        <v-divider></v-divider>

        <v-card-actions class="pa-4 bg-faint">
          <v-spacer></v-spacer>
          <v-btn text @click="dialogPermisos = false" class="rounded-lg">Cancelar</v-btn>
          <v-btn color="primary" class="brand-btn px-8 shadow-premium" @click="guardarPermisos">
            <v-icon left>save</v-icon> Guardar Cambios
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Global Notification -->
    <v-snackbar v-model="snackbar.show" :color="snackbar.color" top right rounded="pill" class="mb-4">
      <div class="d-flex align-center font-weight-bold">
        <v-icon dark class="mr-2">{{ snackbar.color === 'success' ? 'check_circle' : 'error' }}</v-icon>
        {{ snackbar.text }}
      </div>
    </v-snackbar>
  </v-container>
</template>

<script>
import { mapState, mapActions } from "vuex";

export default {
  name: "Usuarios",
  data: () => ({
    dialogPermisos: false,
    validUser: false,
    testingSap: false,
    newUser: { UserName: "", Password: "" },
    selectedUser: "",
    showPasswords: {},
    showNewPass: false,
    filters: { UserName: "" },
    headers: [
      { text: "Usuario", value: "UserName", sortable: true },
      { text: "Contraseña", value: "Password", sortable: false, width: "150px" },
      { text: "Acciones", value: "acciones", align: "end", sortable: false, width: "100px" },
    ],
    snackbar: { show: false, text: "", color: "success" }
  }),
  computed: {
    ...mapState("usuarios", ["usuarios", "loading", "menuTree"]),
    filteredItems() {
       if (!Array.isArray(this.usuarios)) return [];
       // Normalizar para que el filtro funcione con PascalCase aunque venga camelCase
       const normalized = this.usuarios.map(u => ({
         ...u,
         UserName: u.UserName || u.userName,
         Password: u.Password || u.password
       }));
       return normalized.filter(item => {
         return Object.keys(this.filters).every(key => {
           if (!this.filters[key]) return true;
           const val = String(item[key] || '').toLowerCase();
           const filter = String(this.filters[key]).toLowerCase();
           return val.includes(filter);
         });
       });
    },
  },
  mounted() {
    this.getUsuarios();
    this.initFilters();
  },
  methods: {
    ...mapActions("usuarios", [
      "getUsuarios", 
      "createUsuario", 
      "testSapLogin", 
      "getPermissions", 
      "savePermissions"
    ]),

    initFilters() {
      const f = {};
      this.headers.forEach(h => { if(h.value !== 'acciones') f[h.value] = ""; });
      this.filters = f;
    },

    async probarSap() {
      this.testingSap = true;
      try {
        await this.testSapLogin(this.newUser);
        this.mostrarSnack("Login SAP Correcto", "success");
      } catch (error) {
        this.mostrarSnack("Error en Login SAP", "error");
      } finally {
        this.testingSap = false;
      }
    },

    async guardarUsuario() {
      if (!this.$refs.formUser.validate()) return;
      try {
        await this.createUsuario(this.newUser);
        this.mostrarSnack("Usuario creado exitosamente", "success");
        this.newUser = { UserName: "", Password: "" };
        this.$refs.formUser.resetValidation();
        this.getUsuarios();
      } catch (e) {
        this.mostrarSnack(e.response?.data || "Error al crear usuario", "error");
      }
    },

    async abrirPermisos(item) {
      this.selectedUser = item.UserName;
      await this.getPermissions(this.selectedUser);
      this.dialogPermisos = true;
    },

    async guardarPermisos() {
      let permissionsToSave = [];
      this.menuTree.forEach(menu => {
        if(menu.subMenus) {
          menu.subMenus.forEach(sub => {
            if(sub.hasAccess) {
              permissionsToSave.push({
                IdSubMenu: sub.id,
                CanCreate: sub.canCreate || false
              });
            }
          });
        }
      });

      try {
        await this.savePermissions({ 
          username: this.selectedUser, 
          permissions: permissionsToSave 
        });
        this.mostrarSnack("Permisos actualizados correctamente", "success");
        this.dialogPermisos = false;
      } catch (e) {
        this.mostrarSnack("Error al guardar permisos", "error");
      }
    },

    mostrarSnack(text, color) {
      this.snackbar = { show: true, text, color };
    },

    toggleMenuChildren(menu) {
      const state = menu.hasAccess;
      const subs = menu.SubMenus || menu.subMenus || menu.SubMenu || menu.subMenu || [];
      subs.forEach(s => {
        s.hasAccess = state;
        s.HasAccess = state;
      });
    },

    updateChildPermission(menu, sub) {
      sub.HasAccess = sub.hasAccess;
      const subs = menu.SubMenus || menu.subMenus || menu.SubMenu || menu.subMenu || [];
      menu.hasAccess = subs.some(s => s.hasAccess || s.HasAccess);
      menu.HasAccess = menu.hasAccess;
    }
  }
};
</script>

<style scoped>
.usuarios-dashboard { background: #f8fafc !important; min-height: 100vh; }
.theme--dark .usuarios-dashboard { background: #0f172a !important; }

.glass-card {
  background: rgba(255, 255, 255, 0.7) !important;
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.3) !important;
}
.theme--dark .glass-card { background: rgba(30, 30, 30, 0.6) !important; }

.glass-card-panel {
  background: rgba(255, 255, 255, 0.4) !important;
  transition: all 0.3s ease;
}
.glass-card-panel:hover { background: rgba(255, 255, 255, 0.6) !important; }
.theme--dark .glass-card-panel { background: rgba(255, 255, 255, 0.05) !important; }

.shadow-premium { box-shadow: 0 10px 30px -10px rgba(0,0,0,0.1) !important; }
.border-thin { border: 1px solid rgba(0,0,0,0.05) !important; }
.border-left-primary { border-left: 4px solid var(--v-primary-base) !important; }
.border-left-success { border-left: 4px solid #4CAF50 !important; }

.brand-btn {
  background: linear-gradient(135deg, #f8a102 0%, #ffc107 100%) !important;
  color: white !important;
  font-weight: bold !important;
  border-radius: 12px !important;
  text-transform: none !important;
}

.glass-btn-icon { background: rgba(0,0,0,0.03) !important; }
.glass-toolbar { background: rgba(255,255,255,0.6) !important; }
.glass-toolbar-inner { background: rgba(248, 161, 2, 0.05) !important; }

.premium-table >>> thead th {
  background: #fdf5e6 !important;
  color: var(--v-secondary-base) !important;
  font-weight: 800 !important;
  font-size: 0.7rem;
  letter-spacing: 0.5px;
  padding: 8px !important;
  text-transform: uppercase;
}
.theme--dark .premium-table >>> thead th { background: #252525 !important; }

.compact-filter-input >>> .v-input__slot { padding: 0 4px !important; border-radius: 4px !important; min-height: 20px !important; }
.sticky-panel { position: sticky; top: 88px; }
.transparent-panels >>> .v-expansion-panel-header--active { min-height: 48px; }
.bg-selected { background: rgba(248, 161, 2, 0.05) !important; }
.bg-faint { background: rgba(0,0,0,0.01); }
.line-height-tight { line-height: 1.1; }

.submenu-item { transition: background 0.2s ease; }
.submenu-item:hover { background: rgba(0,0,0,0.02); }
</style>
