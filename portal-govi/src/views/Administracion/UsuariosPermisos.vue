<template>
  <v-container fluid class="pa-6 admin-dashboard">
    <!-- Top Action Bar -->
    <v-toolbar dense flat class="glass-toolbar mb-6 rounded-xl elevation-3 border-thin">
      <v-icon color="primary" class="mr-3">mdi-account-key</v-icon>
      <v-toolbar-title class="font-weight-black brand-secondary--text">
        Gestión de <span class="font-weight-light grey--text">Usuarios y Permisos</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      
      <div class="d-flex align-center">
        <v-btn 
          class="brand-btn px-6 shadow-premium mr-2" 
          depressed 
          @click="abrirModalUsuario"
        >
          <v-icon left>mdi-account-plus</v-icon>
          Nuevo Usuario
        </v-btn>

        <v-btn class="glass-btn-icon" icon @click="cargarUsuarios" :loading="loading">
          <v-icon>refresh</v-icon>
        </v-btn>
      </div>
    </v-toolbar>

    <!-- Main Data Table Area -->
    <v-card class="glass-card rounded-xl overflow-hidden shadow-premium border-thin">
      <v-data-table
        :headers="headers"
        :items="filteredUsers"
        class="glass-table premium-table"
        :loading="loading"
        dense
        fixed-header
        height="65vh"
        :items-per-page="50"
        :footer-props="{
          'items-per-page-options': [50, 100, -1]
        }"
      >
        <!-- Custom Headers for Filtering -->
        <template v-for="h in headers" v-slot:[`header.${h.value}`]="{ header }">
          <div :key="h.value" class="d-flex flex-column py-2">
            <span class="mb-1">{{ header.text }}</span>
            <v-text-field
              v-if="h.value !== 'actions'"
              v-model="filters[h.value]"
              dense
              hide-details
              filled
              flat
              solo
              class="compact-filter-input"
              placeholder="Filtro..."
              clearable
            ></v-text-field>
          </div>
        </template>

        <template v-slot:[`item.userName`]="{ item }">
          <div class="d-flex align-center">
            <v-avatar size="32" color="primary lighten-4" class="mr-3">
              <v-icon color="primary" small>mdi-account</v-icon>
            </v-avatar>
            <span class="font-weight-black brand-secondary--text">{{ item.userName }}</span>
          </div>
        </template>

        <template v-slot:[`item.actions`]="{ item }">
          <div class="d-flex justify-end">
            <v-btn 
              color="primary" 
              small 
              depressed 
              class="rounded-lg px-4"
              @click="gestionarPermisos(item)"
            >
              <v-icon left small>mdi-shield-account</v-icon>
              Permisos
            </v-btn>
          </div>
        </template>
      </v-data-table>
    </v-card>

    <!-- MODAL NUEVO USUARIO -->
    <v-dialog v-model="dialogUser" max-width="500px" persistent>
      <v-card class="rounded-xl glass-card border-thin overflow-hidden">
        <v-toolbar flat dense class="glass-toolbar-inner rounded-t-xl">
          <v-icon color="primary" class="mr-2">mdi-account-plus</v-icon>
          <span class="subtitle-1 font-weight-black brand-secondary--text">Nuevo Usuario</span>
          <v-spacer></v-spacer>
          <v-btn icon small @click="dialogUser = false"><v-icon>close</v-icon></v-btn>
        </v-toolbar>

        <v-card-text class="pa-6">
          <v-alert dense text type="info" class="rounded-xl mb-6 text-caption border-thin">
            <v-icon small left color="info">mdi-information</v-icon>
            La contraseña debe coincidir con la de SAP para habilitar operaciones transaccionales.
          </v-alert>
          
          <v-form ref="formUser" v-model="validUser">
            <v-text-field
              v-model="newUser.UserName"
              label="Usuario (ID)"
              filled rounded dense
              prepend-inner-icon="mdi-account-outline"
              class="mb-2"
              required
              :rules="[v => !!v || 'Requerido']"
            ></v-text-field>

            <v-text-field
              v-model="newUser.Password"
              label="Contraseña SAP"
              filled rounded dense
              prepend-inner-icon="mdi-lock-outline"
              type="password"
              class="mb-4"
              required
              :rules="[v => !!v || 'Requerido']"
            ></v-text-field>
          </v-form>

          <v-btn 
            block 
            depressed 
            class="grey lighten-4 grey--text text--darken-2 rounded-xl py-6 mb-4"
            :loading="testingSap" 
            @click="probarSap"
            :disabled="!newUser.UserName || !newUser.Password"
          >
            <v-icon left>mdi-connection</v-icon> Test Login SAP
          </v-btn>
        </v-card-text>

        <v-divider class="mx-6"></v-divider>
        <v-card-actions class="pa-6">
          <v-spacer></v-spacer>
          <v-btn text @click="dialogUser = false" class="rounded-xl px-6">Cancelar</v-btn>
          <v-btn 
            class="brand-btn px-8 shadow-premium" 
            depressed 
            :disabled="!validUser" 
            @click="crearUsuario"
          >
            Crear Usuario
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- MODAL PERMISOS -->
    <v-dialog v-model="dialogPermisos" max-width="800px" scrollable>
      <v-card class="rounded-xl glass-card border-thin overflow-hidden">
        <v-toolbar flat dense class="glass-toolbar-inner rounded-t-xl">
          <v-icon color="primary" class="mr-2">mdi-shield-lock</v-icon>
          <span class="subtitle-1 font-weight-black brand-secondary--text">
            Permisos: <span class="font-weight-light">{{ selectedUser }}</span>
          </span>
          <v-spacer></v-spacer>
          <v-btn icon small @click="dialogPermisos = false"><v-icon>close</v-icon></v-btn>
        </v-toolbar>
        
        <v-card-text class="pa-6" style="height: 60vh;">
          <v-skeleton-loader v-if="loadingPermisos" type="list-item-three-line@3"></v-skeleton-loader>
          
          <div v-else>
             <div class="d-flex align-center mb-6">
                <v-icon color="primary" class="mr-2">mdi-filter-variant</v-icon>
                <span class="text-overline grey--text">Estructura de Menús</span>
             </div>
             
             <v-expansion-panels flat class="rounded-xl overflow-hidden glass-panels border-thin">
               <v-expansion-panel v-for="menu in menuTree" :key="menu.id" class="transparent">
                 <v-expansion-panel-header class="py-4">
                   <div class="d-flex align-center">
                     <v-avatar size="32" color="rgba(248, 161, 2, 0.1)" class="mr-4">
                       <v-icon size="18" color="primary">{{ menu.icon || 'mdi-folder' }}</v-icon>
                     </v-avatar>
                     <span class="font-weight-black grey--text text--darken-3">{{ menu.tag }}</span>
                   </div>
                 </v-expansion-panel-header>
                 <v-expansion-panel-content class="bg-faint">
                   
                   <v-row v-for="sub in menu.subMenus" :key="sub.id" class="align-center mx-0 py-3 border-bottom-thin">
                     <v-col cols="6" class="py-0">
                       <v-checkbox 
                         v-model="sub.tieneAcceso" 
                         :label="sub.tag" 
                         dense hide-details class="mt-0 font-weight-medium"
                         color="primary"
                       ></v-checkbox>
                     </v-col>
                     <v-col cols="6" class="py-0 text-right">
                       <v-switch
                         v-model="sub.canCreate"
                         label="Control de Escritura"
                         dense hide-details class="mt-0 d-inline-flex"
                         :disabled="!sub.tieneAcceso"
                         color="success"
                         inset
                       ></v-switch>
                     </v-col>
                   </v-row>

                 </v-expansion-panel-content>
               </v-expansion-panel>
             </v-expansion-panels>
          </div>
        </v-card-text>
        
        <v-divider class="mx-6"></v-divider>
        <v-card-actions class="pa-6">
          <v-spacer></v-spacer>
          <v-btn text @click="dialogPermisos = false" class="rounded-xl px-6">Cerrar</v-btn>
          <v-btn 
            class="brand-btn px-10 shadow-premium" 
            depressed 
            @click="guardarPermisos"
          >
            Actualizar Permisos
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" rounded="pill" class="mb-4">
      <div class="d-flex align-center font-weight-bold">
        <v-icon dark class="mr-2">{{ snackbar.color === 'success' ? 'mdi-check-circle' : 'mdi-alert' }}</v-icon>
        {{ snackbar.text }}
      </div>
    </v-snackbar>

    <v-overlay :value="loading || testingSap" z-index="200" opacity="0.6">
      <v-progress-circular indeterminate size="70" width="7" color="primary"></v-progress-circular>
    </v-overlay>
  </v-container>
</template>

<script>
import { mapActions } from "vuex";

export default {
  name: "UsuariosPermisos",
  data: () => ({
    loading: false,
    users: [],
    filters: {
      userName: ''
    },
    headers: [
      { text: "Usuario (ID)", value: "userName", sortable: true },
      { text: "Acciones", value: "actions", sortable: false, align: "end", width: "150px" }
    ],

    // Modal Crear
    dialogUser: false,
    validUser: false,
    testingSap: false,
    newUser: { UserName: '', Password: '' },

    // Modal Permisos
    dialogPermisos: false,
    loadingPermisos: false,
    selectedUser: '',
    menuTree: [], 

    snackbar: { show: false, text: '', color: '' }
  }),

  computed: {
    filteredUsers() {
      if (!Array.isArray(this.users)) return [];
      return this.users.filter(user => {
        return Object.keys(this.filters).every(key => {
          if (!this.filters[key]) return true;
          const val = String(user[key] || '').toLowerCase();
          const filter = String(this.filters[key]).toLowerCase();
          return val.includes(filter);
        });
      });
    }
  },

  mounted() {
    this.cargarUsuarios();
  },

  methods: {
    ...mapActions("administracion", [
      "getUsers", "createUser", "testSap", "getPermissionsTree", "savePermissions"
    ]),

    cargarUsuarios() {
      this.loading = true;
      this.getUsers()
        .then(data => this.users = data)
        .catch(err => console.error(err))
        .finally(() => this.loading = false);
    },

    abrirModalUsuario() {
      this.newUser = { UserName: '', Password: '' };
      if (this.$refs.formUser) this.$refs.formUser.resetValidation();
      this.dialogUser = true;
    },

    probarSap() {
      this.testingSap = true;
      this.testSap(this.newUser)
        .then(() => this.msg("Conexión con SAP exitosa", "success"))
        .catch(() => this.msg("Error: Credenciales no válidas en SAP", "error"))
        .finally(() => this.testingSap = false);
    },

    crearUsuario() {
      if(!this.$refs.formUser.validate()) return;
      
      this.createUser(this.newUser)
        .then(() => {
          this.msg("Usuario creado correctamente", "success");
          this.dialogUser = false;
          this.cargarUsuarios();
        })
        .catch(err => this.msg(err.response?.data || "Error al crear", "error"));
    },

    gestionarPermisos(item) {
      this.selectedUser = item.userName;
      this.dialogPermisos = true;
      this.loadingPermisos = true;

      this.getPermissionsTree(this.selectedUser)
        .then(data => {
          this.menuTree = data;
        })
        .catch(err => console.error(err))
        .finally(() => this.loadingPermisos = false);
    },

    guardarPermisos() {
      let permisosSeleccionados = [];
      
      this.menuTree.forEach(menu => {
        menu.subMenus.forEach(sub => {
          if (sub.tieneAcceso) {
            permisosSeleccionados.push({
              IdSubMenu: sub.id,
              CanCreate: sub.canCreate
            });
          }
        });
      });

      const payload = {
        UserName: this.selectedUser,
        Permisos: permisosSeleccionados
      };

      this.savePermissions(payload)
        .then(() => {
          this.msg("Permisos actualizados", "success");
          this.dialogPermisos = false;
        })
        .catch(() => this.msg("Error al guardar permisos", "error"));
    },

    msg(text, color) {
      this.snackbar = { show: true, text, color, timeout: 4000 };
    }
  }
};
</script>

<style scoped>
.admin-dashboard { 
  background: #f8fafc !important; 
  min-height: 100vh; 
}
.theme--dark .admin-dashboard { background: #0f172a !important; }

.glass-card {
  background: rgba(255, 255, 255, 0.7) !important;
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.3) !important;
}
.theme--dark .glass-card { background: rgba(30,30,30, 0.6) !important; }

.glass-toolbar {
  background: rgba(255, 255, 255, 0.7) !important;
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.3) !important;
}
.theme--dark .glass-toolbar { background: rgba(30,30,30, 0.6) !important; }

.shadow-premium { box-shadow: 0 10px 30px -10px rgba(0,0,0,0.1) !important; }
.border-thin { border: 1px solid rgba(0,0,0,0.05) !important; }
.border-bottom-thin { border-bottom: 1px solid rgba(0,0,0,0.05) !important; }

.brand-btn {
  background: linear-gradient(135deg, #f8a102 0%, #ffc107 100%) !important;
  color: white !important;
  font-weight: bold !important;
  border-radius: 12px !important;
  text-transform: none !important;
  letter-spacing: 0.5px;
}

.glass-btn-icon { background: rgba(0,0,0,0.03) !important; }
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

.compact-filter-input >>> .v-input__slot {
  padding: 0 8px !important;
  border-radius: 8px !important;
  min-height: 32px !important;
  background: rgba(0, 0, 0, 0.03) !important;
}
.theme--dark .compact-filter-input >>> .v-input__slot { background: rgba(255, 255, 255, 0.05) !important; }

.glass-panels { background: transparent !important; }
.glass-panels .v-expansion-panel-header--active { background: rgba(248, 161, 2, 0.05) !important; }

.bg-faint { background: rgba(0,0,0,0.015); }
.brand-secondary--text { color: #1e293b !important; }
.theme--dark .brand-secondary--text { color: #f1f5f9 !important; }
</style>
