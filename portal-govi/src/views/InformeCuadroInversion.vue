<template>
  <v-container fluid class="pa-6 inversion-dashboard">
    <!-- Top Action Bar -->
    <v-toolbar dense flat id="p" class="glass-toolbar mb-6 rounded-xl elevation-3 border-thin">
      <v-icon color="primary" class="mr-3">analytics</v-icon>
      <v-toolbar-title class="font-weight-black brand-secondary--text">
        Análisis <span class="font-weight-light grey--text">Cuadro de Inversión</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      
      <div class="d-flex align-center">
        <export-excel
          v-if="data.length > 0"
          :data="data"
          worksheet="Reporte"
          :name="`Inversion_${new Date().toISOString().substr(0,10)}.xls`"
        >
          <v-btn class="brand-btn px-6 shadow-premium mr-2" depressed>
            <v-icon left>mdi-microsoft-excel</v-icon>
            Exportar Excel
          </v-btn>
        </export-excel>

        <v-btn class="glass-btn-icon" icon @click="cargarDatos" :loading="overlay">
          <v-icon>refresh</v-icon>
        </v-btn>
      </div>
    </v-toolbar>

    <!-- Metrics Row -->
    <v-row class="mb-6">
      <v-col cols="12" sm="4">
        <v-card class="metric-card glass-card rounded-xl pa-4 border-left-info shadow-premium">
          <div class="d-flex align-center">
            <v-avatar color="info lighten-4" size="52" class="mr-4">
              <v-icon color="info">business</v-icon>
            </v-avatar>
            <div>
              <div class="text-overline font-weight-black grey--text line-height-tight mb-1">Empresas</div>
              <div class="text-h4 font-weight-black brand-secondary--text">{{ totalEnterprises }}</div>
            </div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="12" sm="4">
        <v-card class="metric-card glass-card rounded-xl pa-4 border-left-success shadow-premium">
          <div class="d-flex align-center">
            <v-avatar color="success lighten-4" size="52" class="mr-4">
              <v-icon color="success">account_balance_wallet</v-icon>
            </v-avatar>
            <div>
              <div class="text-overline font-weight-black grey--text line-height-tight mb-1">Saldo Diario Total</div>
              <div class="text-h4 font-weight-black success--text">{{ totalDailyBalance | currency }}</div>
            </div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="12" sm="4">
        <v-card class="metric-card glass-card rounded-xl pa-4 border-left-primary shadow-premium">
          <div class="d-flex align-center">
            <v-avatar color="primary lighten-4" size="52" class="mr-4">
              <v-icon color="primary">monetization_on</v-icon>
            </v-avatar>
            <div>
              <div class="text-overline font-weight-black grey--text line-height-tight mb-1">Total General</div>
              <div class="text-h4 font-weight-black brand-secondary--text">{{ totalGlobal | currency }}</div>
            </div>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <!-- Config Panel -->
    <v-card class="glass-card mb-6 rounded-xl pa-4 border-thin shadow-sm">
      <div class="d-flex align-center mb-4">
        <v-icon small color="primary" class="mr-2">mdi-cog</v-icon>
        <span class="text-overline font-weight-bold grey--text">Configuración de Consultas</span>
      </div>
      <v-row dense>
        <v-col 
          v-for="(label, index) in dateLabels" 
          :key="index"
          cols="12" sm="4" md="3" lg="1" xl="1"
          class="flex-grow-1"
        >
          <v-menu
            v-model="menus[index]"
            :close-on-content-click="false"
            transition="scale-transition"
            offset-y
            min-width="auto"
          >
            <template v-slot:activator="{ on, attrs }">
              <v-text-field
                v-model="date[index]"
                :label="label"
                prepend-inner-icon="mdi-calendar"
                readonly
                filled
                dense
                hide-details
                v-bind="attrs"
                v-on="on"
                class="rounded-lg compact-field-text"
              ></v-text-field>
            </template>
            <v-date-picker 
              v-model="date[index]" 
              @input="$set(menus, index, false)" 
              locale="es-mx"
              color="primary"
              header-color="secondary"
              class="rounded-xl elevation-5"
            ></v-date-picker>
          </v-menu>
        </v-col>
        <v-col cols="12" md="auto">
          <v-btn class="brand-btn px-6 shadow-premium h-40" depressed @click="cargarDatos" :loading="overlay">
            <v-icon left>mdi-magnify</v-icon>
            Consultar
          </v-btn>
        </v-col>
      </v-row>
    </v-card>

    <!-- Data Table -->
    <v-card class="glass-card rounded-xl overflow-hidden shadow-premium border-thin">
      <v-data-table
        :headers="headers"
        :items="filteredItems"
        :items-per-page="15"
        item-key="cuenta"
        class="glass-table premium-table sticky-first-col"
        fixed-header
        height="55vh" 
        dense
        :loading="overlay"
        loading-text="Calculando saldos..."
        mobile-breakpoint="0" 
      >
        <!-- Custom Headers for Filtering -->
        <template v-for="h in headers" v-slot:[`header.${h.value}`]="{ header }">
          <div :key="h.value" class="d-flex flex-column py-2">
            <span class="mb-1">{{ header.text }}</span>
            <v-text-field
              v-if="!moneyHeaders.concat(['total', 'saldoDiario', 'fecha', 'tipo de Cambio']).includes(h.value)"
              v-model="filters[h.value]"
              dense
              hide-details
              filled
              flat
              solo
              background-color="rgba(0,0,0,0.03)"
              class="compact-filter-input"
              placeholder="Filtro..."
              clearable
            ></v-text-field>
          </div>
        </template>

        <template v-slot:[`item.empresa`]="{ item }">
          <div class="d-flex align-center py-2 px-1">
            <v-avatar size="24" color="primary lighten-4" class="mr-2">
              <span class="text-caption font-weight-bold primary--text">{{ item.empresa.charAt(0) }}</span>
            </v-avatar>
            <span class="font-weight-bold text-truncate" style="max-width: 140px">{{ item.empresa }}</span>
          </div>
        </template>

        <template v-slot:[`item.saldoDiario`]="{ item }">
          <div :class="['font-weight-black', getColor(item.saldoDiario)]"> 
            {{ item.saldoDiario | currency }} 
          </div>
        </template>
        
        <template v-slot:[`item.total`]="{ item }">
           <v-chip dark small color="primary" class="font-weight-black shadow-sm">
             {{ item.total | currency }}
           </v-chip>
        </template>
        
        <template v-for="header in moneyHeaders" v-slot:[`item.${header}`]="{ item }">
           <span :key="header" class="font-weight-medium grey--text text--darken-2">
             {{ item[header] | currency }}
           </span>
        </template>

        <template v-slot:[`item.nombreCuenta`]="{ item }">
          <span class="text-caption font-weight-medium">{{ item.nombreCuenta }}</span>
        </template>

      </v-data-table>
    </v-card>

    <v-overlay :value="overlay" z-index="9999">
      <div class="text-center">
        <v-progress-circular indeterminate size="64" color="primary" class="mb-3"></v-progress-circular>
        <h3 class="font-weight-bold primary--text">Procesando Cuadro de Inversión</h3>
        <p class="grey--text">Estamos organizando los datos financieros...</p>
      </div>
    </v-overlay>
  </v-container>
</template>

<script>
import { mapActions } from "vuex";

export default {
  data() {
    return {
      search: "",
      menus: [false, false, false, false, false, false, false],
      dateLabels: [
        "F. Inicio Cuadro", "F. Fin Cuadro", "F. Saldo Diario", 
        "F. Ini Doc.", "F. Fin Doc.", "F. Ini Contab.", "F. Fin Contab."
      ],
      date: Array(7).fill(new Date().toISOString().substr(0, 10)),
      data: [],
      overlay: false,
      filters: {
        empresa: "",
        cuenta: "",
        nombreCuenta: "",
        tesoreria: ""
      },
      // Lista de campos que son moneda para iterar en el template (ahorra codigo)
      moneyHeaders: [
         "transfers", "importe Sin Domiciliados", "importe Domiciliado",
         "domiciliados EDC (CFE,DOMICILIACION)", "comisiones EDC (IVA,COMISION)",
         "depositos", "depositos Cuenta Propia", "cheques Devolucion"
      ],
      headers: [
        { text: "Empresa", value: "empresa", width: "160px", fixed: true },
        { text: "Cuenta", value: "cuenta", width: "130px" },
        { text: "Nombre Cuenta", value: "nombreCuenta", width: "220px" },
        { text: "Saldo Diario", value: "saldoDiario", align: "end", width: "140px" },
        { text: "Total", value: "total", align: "end", width: "140px" },
        { text: "Transfers", value: "transfers", align: "end", width: "130px" },
        { text: "Imp. Sin Dom.", value: "importe Sin Domiciliados", align: "end", width: "130px" },
        { text: "Imp. Dom.", value: "importe Domiciliado", align: "end", width: "130px" },
        { text: "Dom. EDC", value: "domiciliados EDC (CFE,DOMICILIACION)", align: "end", width: "140px" },
        { text: "Com. EDC", value: "comisiones EDC (IVA,COMISION)", align: "end", width: "140px" },
        { text: "Depósitos", value: "depositos", align: "end", width: "130px" },
        { text: "Dep. Cta Propia", value: "depositos Cuenta Propia", align: "end", width: "150px" },
        { text: "Chq. Dev.", value: "cheques Devolucion", align: "end", width: "130px" },
        { text: "Tesorería", value: "tesoreria", width: "130px" },
        { text: "T.C.", value: "tipo de Cambio", width: "80px" },
        { text: "Fecha", value: "fecha", width: "110px" },
      ],
    };
  },
  filters: {
    currency(value) {
      if (!value) return "-"; // Muestra un guion si es 0 o null para limpiar la vista
      return new Intl.NumberFormat("es-MX", {
        style: "currency",
        currency: "MXN",
        minimumFractionDigits: 2
      }).format(value);
    }
  },
  methods: {
    ...mapActions("informes", { getInfo: "getCuadroInversion" }),
    
    getColor(amount) {
      if (amount < 0) return 'red--text text--accent-4';
      return 'green--text text--darken-3';
    },

    cargarDatos() {
      const fechas = this.date.map((d) => d.replaceAll("-", "")).join(",");
      this.overlay = true;
      this.getInfo(fechas)
        .then((data) => {
          this.data = data;
        })
        .catch((error) => console.error(error))
        .finally(() => {
          this.overlay = false;
        });
    },
  },
  computed: {
    totalEnterprises() {
      if (!Array.isArray(this.data)) return 0;
      return new Set(this.data.map(item => item.empresa)).size;
    },
    totalDailyBalance() {
      if (!Array.isArray(this.filteredItems)) return 0;
      return this.filteredItems.reduce((acc, item) => acc + (parseFloat(item.saldoDiario) || 0), 0);
    },
    totalGlobal() {
      if (!Array.isArray(this.filteredItems)) return 0;
      return this.filteredItems.reduce((acc, item) => acc + (parseFloat(item.total) || 0), 0);
    },
    filteredItems() {
      if (!Array.isArray(this.data)) return [];
      return this.data.filter(item => {
        return Object.keys(this.filters).every(key => {
          if (!this.filters[key]) return true;
          const val = String(item[key] || '').toLowerCase();
          const filter = String(this.filters[key]).toLowerCase();
          return val.includes(filter);
        });
      });
    }
  }
};
</script>

<style scoped>
.inversion-dashboard {
  background: var(--v-background-base);
  min-height: 100vh;
}

.glass-card {
  background: rgba(255, 255, 255, 0.7) !important;
  backdrop-filter: blur(12px) saturate(180%);
  -webkit-backdrop-filter: blur(12px) saturate(180%);
  border: 1px solid rgba(255, 255, 255, 0.3) !important;
  transition: all 0.3s ease;
}

.theme--dark .glass-card {
  background: rgba(30,30,30, 0.6) !important;
  border: 1px solid rgba(255, 255, 255, 0.1) !important;
}

.shadow-premium {
  box-shadow: 0 10px 30px -10px rgba(0,0,0,0.1) !important;
}

.border-thin {
  border: 1px solid rgba(0,0,0,0.05) !important;
}

.border-left-primary { border-left: 5px solid var(--v-primary-base) !important; }
.border-left-success { border-left: 5px solid #4CAF50 !important; }
.border-left-info { border-left: 5px solid #2196F3 !important; }

.metric-card {
  transition: transform 0.3s ease;
}

.metric-card:hover {
  transform: translateY(-5px);
}

.brand-btn {
  background: linear-gradient(135deg, #f8a102 0%, #ffc107 100%) !important;
  color: white !important;
  font-weight: bold !important;
  border-radius: 12px !important;
  text-transform: none !important;
  letter-spacing: 0.5px;
}

.h-40 {
  height: 40px !important;
}

.compact-field-text >>> .v-input__control {
  min-height: 40px !important;
}

.compact-field-text >>> .v-label {
  font-size: 0.75rem !important;
}

.premium-table >>> .v-data-table__wrapper table {
  font-size: 0.825rem;
}

.premium-table >>> thead th {
  background: #fdf5e6 !important;
  color: var(--v-secondary-base) !important;
  font-weight: 800 !important;
  text-transform: uppercase;
  font-size: 0.725rem;
  letter-spacing: 0.5px;
  padding-top: 4px !important;
  padding-bottom: 4px !important;
  height: auto !important;
  z-index: 10 !important;
}

.theme--dark .premium-table >>> thead th {
  background: #252525 !important;
}

.compact-filter-input >>> .v-input__control {
  min-height: 20px !important;
  font-size: 0.65rem !important;
}

.compact-filter-input >>> .v-input__slot {
  padding: 0 4px !important;
  border-radius: 4px !important;
  min-height: 20px !important;
}

.compact-filter-input >>> input {
  padding: 0 !important;
}

/* Sticky Column Improvements */
.sticky-first-col >>> table > thead > tr > th:first-child,
.sticky-first-col >>> table > tbody > tr > td:first-child {
  position: sticky !important; 
  left: 0;
  z-index: 11 !important;
  background: white !important; 
  border-right: 2px solid rgba(0,0,0,0.1) !important;
}

.theme--dark .sticky-first-col >>> table > thead > tr > th:first-child,
.theme--dark .sticky-first-col >>> table > tbody > tr > td:first-child {
  background: #1e1e1e !important;
  border-right: 2px solid rgba(255,255,255,0.1) !important;
}

.premium-table >>> tbody tr:hover td {
  background-color: rgba(248, 161, 2, 0.05) !important;
}

.line-height-tight {
  line-height: 1.2;
}

.h-40 { height: 40px !important; }
</style>
