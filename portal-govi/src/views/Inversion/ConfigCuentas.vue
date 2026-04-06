<template>
  <v-container fluid class="pa-6 cuentas-dashboard">
    <!-- Top Action Bar -->
    <v-toolbar dense flat class="glass-toolbar mb-6 rounded-xl elevation-3 border-thin">
      <v-icon color="primary" class="mr-3">mdi-bank</v-icon>
      <v-toolbar-title class="font-weight-black brand-secondary--text">
        Configuración de <span class="font-weight-light grey--text">Cuentas Bancarias</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      
      <div class="d-flex align-center">
        <v-btn 
          class="brand-btn px-6 shadow-premium mr-2" 
          depressed 
          @click="dialogPrincipal = true"
        >
          <v-icon left>mdi-plus</v-icon>
          Nueva Principal
        </v-btn>

        <v-btn class="glass-btn-icon" icon @click="getPrincipales" :loading="loading">
          <v-icon>refresh</v-icon>
        </v-btn>
      </div>
    </v-toolbar>

    <v-row class="fill-height">
      <!-- PANEL MAESTRO (Izquierda) -->
      <v-col cols="12" md="5" lg="4">
        <v-card class="glass-card rounded-xl overflow-hidden shadow-premium border-thin flex-column d-flex" style="height: calc(100vh - 160px);">
          <div class="pa-4 glass-toolbar-inner border-bottom-thin">
            <v-text-field
              v-model="search"
              prepend-inner-icon="mdi-magnify"
              label="Buscador de cuentas"
              hide-details
              filled rounded dense
              class="account-search-bar"
            ></v-text-field>
          </div>

          <v-list two-line class="overflow-y-auto py-0 transparent flex-grow-1">
            <v-list-item-group v-model="selectedItem" color="primary" mandatory>
              <template v-for="(item, index) in filteredPrincipales">
                <v-list-item 
                  :key="item.orden" 
                  @click="selectPrincipal(item)"
                  class="account-item mx-2 my-1 rounded-lg"
                >
                  <v-list-item-avatar color="primary lighten-4">
                    <v-icon color="primary" small>mdi-bank-outline</v-icon>
                  </v-list-item-avatar>
                  <v-list-item-content>
                    <v-list-item-title class="font-weight-black grey--text text--darken-3 text-truncate">{{ item.almacen }}</v-list-item-title>
                    <v-list-item-subtitle class="caption grey--text">{{ item.razonSocial }}</v-list-item-subtitle>
                    <v-list-item-subtitle class="primary--text font-weight-bold caption">{{ item.cuenta }}</v-list-item-subtitle>
                  </v-list-item-content>
                  <v-list-item-action>
                    <v-chip x-small dark color="primary" class="font-weight-black">#{{ item.orden }}</v-chip>
                  </v-list-item-action>
                </v-list-item>
                <v-divider :key="`div-${index}`" v-if="index < filteredPrincipales.length - 1" class="mx-4 opacity-10"></v-divider>
              </template>
            </v-list-item-group>
          </v-list>
          
          <v-overlay v-if="loading" absolute color="white" opacity="0.5">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </v-overlay>
        </v-card>
      </v-col>

      <!-- PANEL DETALLE (Derecha) -->
      <v-col cols="12" md="7" lg="8">
        <v-fade-transition mode="out-in">
          <v-container v-if="!selectedAccount" fluid class="fill-height d-flex align-center justify-center">
            <div class="text-center pa-10 glass-card rounded-xl border-thin shadow-premium">
              <v-avatar color="primary lighten-4" size="120" class="mb-6">
                 <v-icon size="64" color="primary">mdi-card-search-outline</v-icon>
              </v-avatar>
              <div class="text-h5 font-weight-black brand-secondary--text">Selecciona una cuenta</div>
              <div class="subtitle-1 grey--text mt-2">Gestiona dependencias USD y Referencias</div>
            </div>
          </v-container>

          <div v-else class="fill-height d-flex flex-column">
            <!-- Header Detalle -->
            <v-card class="glass-card rounded-xl pa-6 border-thin shadow-premium mb-6 overflow-hidden position-relative">
              <div class="d-flex align-center">
                <v-avatar color="primary" size="64" class="elevation-4 mr-6">
                  <v-icon dark size="32">mdi-office-building</v-icon>
                </v-avatar>
                <div>
                  <div class="text-h4 font-weight-black brand-secondary--text line-height-tight mb-1">{{ selectedAccount.almacen }}</div>
                  <div class="subtitle-1 grey--text font-weight-medium">{{ selectedAccount.razonSocial }}</div>
                </div>
                <v-spacer></v-spacer>
                <div class="text-right">
                  <div class="text-overline grey--text mb-1">CLABE INTERBANCARIA</div>
                  <v-chip color="primary" label class="rounded-lg font-weight-black px-4" dark>
                    {{ selectedAccount.cuenta }}
                  </v-chip>
                </div>
              </div>

              <v-tabs v-model="tab" color="primary" class="mt-8 rounded-xl elevation-2 white overflow-hidden" grow>
                <v-tab class="text-none font-weight-black py-4">
                  <v-icon left>mdi-currency-usd</v-icon> Cuentas USD
                </v-tab>
                <v-tab class="text-none font-weight-black py-4">
                  <v-icon left>mdi-link-variant</v-icon> Referencias
                </v-tab>
              </v-tabs>
            </v-card>

            <v-tabs-items v-model="tab" class="transparent">
              <v-tab-item v-for="type in ['usd', 'ref']" :key="type">
                <v-card class="glass-card rounded-xl overflow-hidden shadow-premium border-thin">
                  <v-data-table
                    :headers="depHeaders"
                    :items="filteredDependencias(type)"
                    class="glass-table premium-table"
                    hide-default-footer
                    disable-pagination
                    dense
                  >
                    <template v-slot:top>
                      <v-toolbar flat class="glass-toolbar-inner border-bottom-thin">
                        <v-icon small color="primary" class="mr-2">mdi-format-list-bulleted</v-icon>
                        <v-toolbar-title class="subtitle-1 font-weight-black brand-secondary--text">
                          {{ type === 'usd' ? 'Cuentas en Dólares' : 'Referencias Bancarias' }}
                        </v-toolbar-title>
                        <v-spacer></v-spacer>
                        <v-btn color="primary" small depressed class="rounded-lg px-4" @click="openDialogDep(type === 'usd' ? 'usd' : 'referenciada')">
                          <v-icon left small>mdi-plus</v-icon> Agregar
                        </v-btn>
                      </v-toolbar>
                    </template>

                    <!-- Custom Filters -->
                    <template v-for="h in depHeaders" v-slot:[`header.${h.value}`]="{ header }">
                      <div :key="h.value" class="d-flex flex-column py-2">
                        <span class="mb-1">{{ header.text }}</span>
                        <v-text-field
                          v-model="depFilters[h.value]"
                          dense hide-details filled flat solo
                          background-color="rgba(0,0,0,0.03)"
                          class="compact-filter-input"
                          placeholder="..."
                          clearable
                        ></v-text-field>
                      </div>
                    </template>

                    <template v-slot:[`item.almacen`]="{ item }">
                      <span class="font-weight-black brand-secondary--text">{{ item.almacen }}</span>
                    </template>
                    <template v-slot:[`item.cuenta`]="{ item }">
                      <v-chip x-small outlined color="primary" class="font-weight-bold">{{ item.cuenta }}</v-chip>
                    </template>
                  </v-data-table>
                </v-card>
              </v-tab-item>
            </v-tabs-items>
          </div>
        </v-fade-transition>
      </v-col>
    </v-row>

    <!-- DIALOGS -->
    <v-dialog v-model="dialogPrincipal" max-width="500">
      <v-card class="rounded-xl glass-card border-thin overflow-hidden">
        <v-toolbar flat dense class="glass-toolbar-inner">
          <v-icon color="primary" class="mr-2">mdi-bank-plus</v-icon>
          <span class="subtitle-1 font-weight-black brand-secondary--text">Nueva Cuenta Principal</span>
          <v-spacer></v-spacer>
          <v-btn icon small @click="dialogPrincipal = false"><v-icon>close</v-icon></v-btn>
        </v-toolbar>
        
        <v-card-text class="pa-6">
          <v-text-field v-model="formPrincipal.almacen" label="Almacén *" filled rounded dense class="mb-2" hide-details></v-text-field>
          <v-text-field v-model="formPrincipal.razonSocial" label="Razón Social *" filled rounded dense class="mb-2" hide-details></v-text-field>
          <v-text-field v-model="formPrincipal.cuenta" label="CLABE / Cuenta *" filled rounded dense class="mb-0" hide-details></v-text-field>
        </v-card-text>
        <v-divider class="mx-6"></v-divider>
        <v-card-actions class="pa-6">
          <v-spacer></v-spacer>
          <v-btn text @click="dialogPrincipal = false" class="rounded-xl px-6">Cancelar</v-btn>
          <v-btn class="brand-btn px-8 shadow-premium" depressed @click="savePrincipal" :loading="loading">Guardar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="dialogDep" max-width="500">
      <v-card class="rounded-xl glass-card border-thin overflow-hidden">
        <v-toolbar flat dense class="glass-toolbar-inner">
          <v-icon color="primary" class="mr-2">mdi-plus-box-outline</v-icon>
          <span class="subtitle-1 font-weight-black brand-secondary--text">
            {{ currentDepType === 'usd' ? 'Nueva Cuenta USD' : 'Nueva Referencia' }}
          </span>
          <v-spacer></v-spacer>
          <v-btn icon small @click="dialogDep = false"><v-icon>close</v-icon></v-btn>
        </v-toolbar>
        
        <v-card-text class="pa-6">
          <v-text-field v-model="formDep.almacen" label="Almacén *" filled rounded dense class="mb-2" hide-details></v-text-field>
          <v-text-field v-model="formDep.razonSocial" label="Razón Social *" filled rounded dense class="mb-2" hide-details></v-text-field>
          <v-text-field v-model="formDep.cuenta" label="Número de Cuenta *" filled rounded dense class="mb-0" hide-details></v-text-field>
        </v-card-text>
        <v-divider class="mx-6"></v-divider>
        <v-card-actions class="pa-6">
          <v-spacer></v-spacer>
          <v-btn text @click="dialogDep = false" class="rounded-xl px-6">Cancelar</v-btn>
          <v-btn class="brand-btn px-10 shadow-premium" depressed @click="saveDep" :loading="loading">Confirmar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" rounded="pill" class="mb-4">
      <div class="d-flex align-center font-weight-bold">
        <v-icon dark class="mr-2">{{ snackbar.color === 'success' ? 'mdi-check-circle' : 'mdi-alert' }}</v-icon>
        {{ snackbar.text }}
      </div>
    </v-snackbar>
  </v-container>
</template>

<script>
import { mapState, mapActions } from "vuex";

export default {
  name: "ConfigCuentas",
  data: () => ({
    search: "",
    selectedItem: null,
    selectedAccount: null,
    tab: null,
    dialogPrincipal: false,
    dialogDep: false,
    currentDepType: 'usd',
    formPrincipal: { almacen: '', razonSocial: '', cuenta: '' },
    formDep: { almacen: '', razonSocial: '', cuenta: '' },
    depFilters: { almacen: '', razonSocial: '', cuenta: '' },
    depHeaders: [
      { text: "Almacén", value: "almacen", sortable: true },
      { text: "Razón Social", value: "razonSocial", sortable: true },
      { text: "Cuenta", value: "cuenta", sortable: true, width: '150px' },
    ],
    snackbar: { show: false, text: '', color: 'success' }
  }),
  computed: {
    ...mapState("configCuentas", ["principales", "dependencias", "loading"]),
    filteredPrincipales() {
      let filtered = this.principales;
      if (this.search) {
        const s = this.search.toLowerCase();
        filtered = filtered.filter(p => 
          (p.almacen || '').toLowerCase().includes(s) || 
          (p.razonSocial || '').toLowerCase().includes(s) || 
          (p.cuenta || '').toLowerCase().includes(s)
        );
      }
      return [...filtered].sort((a, b) => (a.orden || 0) - (b.orden || 0));
    }
  },
  methods: {
    ...mapActions("configCuentas", ["getPrincipales", "addPrincipal", "loadDependencias", "addDependencia"]),
    
    filteredDependencias(type) {
      const items = type === 'usd' ? this.dependencias.usd : this.dependencias.ref;
      if (!items) return [];
      return items.filter(item => {
        return Object.keys(this.depFilters).every(key => {
          if (!this.depFilters[key]) return true;
          const val = String(item[key] || '').toLowerCase();
          const filter = String(this.depFilters[key]).toLowerCase();
          return val.includes(filter);
        });
      });
    },

    selectPrincipal(item) {
      this.selectedAccount = item;
      this.loadDependencias(item.orden);
    },

    async savePrincipal() {
      if (!this.formPrincipal.almacen || !this.formPrincipal.cuenta || !this.formPrincipal.razonSocial) {
        this.msg("Completa los campos obligatorios", "error");
        return;
      }
      try {
        await this.addPrincipal(this.formPrincipal);
        this.dialogPrincipal = false;
        this.formPrincipal = { almacen: '', razonSocial: '', cuenta: '' };
        this.msg("Cuenta principal guardada", "success");
      } catch (e) { 
        this.msg("Error al guardar", "error");
      }
    },

    openDialogDep(type) {
      this.currentDepType = type;
      this.dialogDep = true;
    },

    async saveDep() {
      if (!this.formDep.almacen || !this.formDep.cuenta || !this.formDep.razonSocial) {
        this.msg("Completa los campos obligatorios", "error");
        return;
      }
      try {
        await this.addDependencia({ type: this.currentDepType, payload: this.formDep });
        this.dialogDep = false;
        this.formDep = { almacen: '', razonSocial: '', cuenta: '' };
        this.msg("Dependencia agregada", "success");
      } catch (e) { 
        this.msg("Error al guardar", "error");
      }
    },

    msg(text, color) {
      this.snackbar = { show: true, text, color };
    }
  },
  created() {
    this.getPrincipales();
    // Iniciar filtros vacios
    const f = {};
    this.depHeaders.forEach(h => f[h.value] = "");
    this.depFilters = f;
  }
};
</script>

<style scoped>
.cuentas-dashboard { 
  background: #f8fafc !important; 
  min-height: 100vh; 
}
.theme--dark .cuentas-dashboard { background: #0f172a !important; }

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

.account-item { border-left: 4px solid transparent; transition: all 0.2s; }
.account-item.v-list-item--active { 
  background: rgba(248, 161, 2, 0.08) !important; 
  border-left: 4px solid #f8a102 !important;
}

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

.account-search-bar >>> .v-input__slot { background: rgba(255, 255, 255, 0.9) !important; }
.theme--dark .account-search-bar >>> .v-input__slot { background: rgba(255, 255, 255, 0.05) !important; }
.account-search-bar >>> input { font-weight: 500 !important; }

.brand-secondary--text { color: #1e293b !important; }
.theme--dark .brand-secondary--text { color: #f1f5f9 !important; }
.line-height-tight { line-height: 1.1; }
.opacity-10 { opacity: 0.1; }
</style>
