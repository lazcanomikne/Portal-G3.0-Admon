<template>
  <v-container fluid class="pa-6 disponible-dashboard">
    <!-- Top Action Bar -->
    <v-toolbar dense flat id="p" class="glass-toolbar mb-6 rounded-xl elevation-3 border-thin">
      <v-icon color="primary" class="mr-3">account_balance_wallet</v-icon>
      <v-toolbar-title class="font-weight-black brand-secondary--text">
        Saldo <span class="font-weight-light grey--text">Disponible</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      
      <div class="d-flex align-center">
        <v-btn 
          class="brand-btn px-6 shadow-premium mr-2" 
          depressed 
          :disabled="!selectedFile" 
          @click="EnviarSap"
          :loading="overlay"
        >
          <v-icon left>mdi-send</v-icon>
          Enviar
        </v-btn>

        <v-btn 
          v-if="id > 0" 
          class="glass-btn-icon mr-2" 
          icon 
          color="error" 
          @click="BorrarRegistros"
          :loading="overlay"
        >
          <v-icon>delete_sweep</v-icon>
        </v-btn>

        <v-btn class="glass-btn-icon" icon @click="OnLoadData" :loading="overlay">
          <v-icon>refresh</v-icon>
        </v-btn>
      </div>
    </v-toolbar>

    <!-- Config Card -->
    <v-card class="glass-card mb-6 rounded-xl pa-4 border-thin shadow-sm">
      <v-row dense align="center">
        <v-col cols="12" sm="4" md="3">
          <v-text-field 
            v-model="fecha" 
            label="Fecha" 
            prepend-inner-icon="event" 
            type="date" 
            filled 
            dense
            hide-details
            class="rounded-lg"
            @input="OnLoadData"
          ></v-text-field>
        </v-col>
        <v-col cols="12" sm="8" md="9">
          <v-file-input 
            v-model="selectedFile"
            label="Cargar archivo de saldos (Excel)" 
            filled 
            dense 
            hide-details 
            show-size 
            prepend-inner-icon="mdi-file-excel"
            class="rounded-lg"
            @change="onFileChange"
          ></v-file-input>
        </v-col>
      </v-row>
    </v-card>

    <!-- Main Data Table Area -->
    <v-card class="glass-card rounded-xl overflow-hidden shadow-premium border-thin">
      <v-data-table
        :headers="columns"
        :items="filteredItems"
        class="glass-table premium-table"
        :loading="overlay"
        dense
        fixed-header
        height="60vh"
        :items-per-page="50"
        :footer-props="{
          'items-per-page-options': [50, 100, -1]
        }"
      >
        <!-- Custom Headers for Filtering -->
        <template v-for="h in columns" v-slot:[`header.${h.value}`]="{ header }">
          <div :key="h.value" class="d-flex flex-column py-2">
            <span class="mb-1">{{ header.text }}</span>
            <v-text-field
              v-if="!['ID', 'id', 'iD_Encabezado', 'iD_Detalle'].includes(h.value)"
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

        <template v-slot:no-data>
          <div class="text-center pa-10">
            <v-img 
              src="https://cdn.vuetifyjs.com/images/cards/foster.jpg" 
              height="100" 
              contain 
              class="mb-4 opacity-50 gray-scale"
            ></v-img>
            <v-icon size="64" color="grey lighten-2">mdi-database-off</v-icon>
            <p class="text-h6 grey--text text--lighten-1 mt-4">No hay información disponible para esta fecha</p>
            <p class="text-body-2 grey--text text--lighten-2">Cargue un archivo para comenzar</p>
          </div>
        </template>
        
        <!-- Money Columns Formatting -->
        <template v-for="col in moneyFields" v-slot:[`item.${col}`]="{ item }">
          <span :key="col" :class="['font-weight-black', getMoneyColor(item[col])]">
            {{ item[col] | currency }}
          </span>
        </template>

        <template v-slot:[`item.CUENTA`]="{ item }">
          <span class="font-weight-medium grey--text text--darken-2">{{ item.CUENTA }}</span>
        </template>

        <template v-slot:[`item.CLABE`]="{ item }">
          <span class="text-caption grey--text">{{ item.CLABE }}</span>
        </template>

        <template v-slot:[`item.fecha`]="{ item }">
          {{ item.fecha | formatDate }}
        </template>

        <template v-slot:[`item.FECHA`]="{ item }">
          {{ item.FECHA | formatDate }}
        </template>
      </v-data-table>
    </v-card>

    <!-- Success Dialog -->
    <v-dialog v-model="showAlert" max-width="400" persistent>
      <v-card class="rounded-xl pa-4 text-center glass-card">
        <v-avatar color="success lighten-4" size="70" class="mb-4">
          <v-icon color="success" size="40">mdi-check-circle</v-icon>
        </v-avatar>
        <v-card-title class="headline justify-center font-weight-black success--text">
          ¡Éxito!
        </v-card-title>
        <v-card-text class="text-body-1 grey--text text--darken-2">
          El proceso de envío a SAP ha finalizado correctamente.
        </v-card-text>
        <v-card-actions class="justify-center pt-4">
          <v-btn 
            class="brand-btn px-10" 
            depressed 
            @click="showAlert = false; response = [];"
          >
            Aceptar
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Processing Overlay -->
    <v-overlay :value="overlay" z-index="200" opacity="0.6">
      <div class="text-center">
        <v-progress-circular indeterminate size="70" width="7" color="primary" class="mb-4">
          <v-avatar size="40">
            <v-img src="https://portal.govi.com.mx/img/logo.png" contain></v-img>
          </v-avatar>
        </v-progress-circular>
        <p class="text-h6 font-weight-bold white--text">Procesando información...</p>
        <p class="text-body-2 white--text opacity-70">Esto puede tomar unos segundos</p>
      </div>
    </v-overlay>
  </v-container>
</template>

<script>
import moment from "moment";
import { mapActions, mapState } from "vuex";
import xlsx from "xlsx";
import { mixin } from "../../mixin";

export default {
  name: "SaldoDisponible",
  data: () => ({
    fecha: moment().format("YYYY-MM-DD"),
    selectedFile: undefined,
    overlay: false,
    response: [],
    showAlert: false,
    id: 0,
    columns: [],
    rows: [],
    filters: {},
    moneyFields: ['SALDO ACTUAL', 'SALDO DISPONIBLE', 'SALDO RETENIDO', 'saldoActual', 'saldoDisponible', 'saldoRetenido']
  }),
  mixins: [mixin],
  filters: {
    currency(value) {
      if (!value && value !== 0) return "-";
      const val = typeof value === 'string' ? parseFloat(value.replace(/[^0-9.-]+/g,"")) : value;
      if (isNaN(val)) return value;
      
      return new Intl.NumberFormat("es-MX", {
        style: "currency",
        currency: "MXN",
      }).format(val);
    },
    formatDate(value) {
      if (!value) return "-";
      return moment(value).format("YYYY-MM-DD");
    }
  },
  mounted () {
    this.OnLoadData()
  },
  methods: {
    ...mapActions("inversion", ["postSaldoDisponible", "getSaldoDisponible", "deleteSaldoDisponible"]),
    
    getMoneyColor(val) {
      const num = typeof val === 'string' ? parseFloat(val.replace(/[^0-9.-]+/g,"")) : val;
      return num < 0 ? 'red--text' : 'green--text text--darken-2';
    },

    EnviarSap () {
      this.overlay = true;
      const data = {
        Fecha: this.fecha,
        Archivo: this.selectedFile ? this.selectedFile.name : 'Archivo_Cargado',
        Usuario: this.userName,
        Detalle: this.rows.map(item => {
          return {
            Fecha: moment(this.fecha).format("YYYY-MM-DD"),
            Clabe: item['CLABE'] || item['clabe'],
            Cuenta: item['CUENTA'] || item['cuenta'],
            Moneda: item['MONEDA'] || item['moneda'],
            SaldoActual: item['SALDO ACTUAL'] || item['saldoActual'],
            SaldoDisponible: item['SALDO DISPONIBLE'] || item['saldoDisponible'],
            SaldoRetenido: item['SALDO RETENIDO'] || item['saldoRetenido'],
            Titular: item['TITULAR / PERSONALIZACIÓN'] || item['titular'],
          }
        })
      }
      this.postSaldoDisponible(data)
        .then((res) => {
          if (res) {
            this.overlay = false;
            this.rows = [];
            this.selectedFile = undefined;
            this.response = res.data;
            this.showAlert = true;
          }
        })
        .catch((err) => {
          this.overlay = false;
          alert("Error al enviar a SAP: " + (err.data || err));
          console.error(err);
        })
        .finally(() => {
          this.overlay = false;
        });
    },
    onFileChange () {
      if (!this.selectedFile) {
        this.rows = [];
        return;
      }
      if (!/\.(xls|xlsx)$/.test(this.selectedFile.name.toLowerCase())) {
        return alert("Formato incorrecto. Use XLS o XLSX.");
      }
      const fileReader = new FileReader();
      fileReader.onload = (ev) => {
        try {
          const data = ev.target.result;
          const workbook = xlsx.read(data, { type: "array" });
          const wsname = workbook.SheetNames[0];
          const ws = xlsx.utils.sheet_to_json(workbook.Sheets[wsname]);
          
          if (ws.length > 0) {
            const firstRow = ws[0];
            this.columns = Object.keys(firstRow)
              .filter(k => !['ID', 'id', 'iD_Encabezado', 'iD_Detalle'].includes(k))
              .map(k => ({
                text: k.toUpperCase(),
                value: k
              }));
            this.rows = ws;
            this.initFilters();
          }
        } catch (e) {
          console.error(e);
          return alert("Error al leer el archivo!");
        }
      };
      fileReader.readAsArrayBuffer(this.selectedFile);
    },
    OnLoadData () {
      this.overlay = true;
      this.getSaldoDisponible(this.fecha)
        .then((res) => {
          if (res && res.length > 0) {
            this.id = res[0].iD_Encabezado || 0;
            this.columns = Object.keys(res[0])
              .filter(k => !['ID', 'id', 'iD_Encabezado', 'iD_Detalle'].includes(k))
              .map(k => ({
                text: k.replace(/([A-Z])/g, ' $1').trim().toUpperCase(),
                value: k
              }));
            this.rows = res;
            this.initFilters();
          } else {
            this.id = 0;
            this.rows = [];
            this.columns = [];
          }
        })
        .catch((err) => {
          console.error(err);
        })
        .finally(() => {
          this.overlay = false;
        });
    },
    BorrarRegistros () {
      if (!confirm("¿Está seguro de eliminar los registros de esta fecha?")) return;
      this.overlay = true;
      this.deleteSaldoDisponible(this.id)
        .then((res) => {
          if (res) {
            this.rows = [];
            this.columns = [];
            this.id = 0;
          }
        })
        .catch((err) => {
          alert("Error al borrar: " + err);
        })
        .finally(() => {
          this.overlay = false;
        });
    },
    initFilters() {
      const newFilters = {};
      this.columns.forEach(col => {
        newFilters[col.value] = "";
      });
      this.filters = newFilters;
    }
  },
  computed: {
    ...mapState("login", ["userName"]),
    filteredItems() {
      if (!Array.isArray(this.rows)) return [];
      return this.rows.filter(item => {
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
.disponible-dashboard {
  background: #f8fafc !important; 
  min-height: 100vh;
}
.theme--dark .disponible-dashboard { 
  background: #0f172a !important; 
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

.brand-btn {
  background: linear-gradient(135deg, #f8a102 0%, #ffc107 100%) !important;
  color: white !important;
  font-weight: bold !important;
  border-radius: 12px !important;
  text-transform: none !important;
  letter-spacing: 0.5px;
}

.brand-btn.v-btn--disabled {
  background: #eee !important;
  color: #aaa !important;
}

.glass-btn-icon {
  background: rgba(0,0,0,0.03) !important;
  transition: all 0.2s;
}

.glass-btn-icon:hover {
  background: rgba(0,0,0,0.08) !important;
  transform: scale(1.1);
}

.premium-table >>> .v-data-table__wrapper table {
  font-size: 0.825rem;
}

.premium-table >>> thead th {
  background: #fdf5e6 !important;
  color: var(--v-secondary-base) !important;
  font-weight: 800 !important;
  text-transform: uppercase;
  font-size: 0.7rem;
  letter-spacing: 0.5px;
  padding-top: 4px !important;
  padding-bottom: 4px !important;
  height: auto !important;
  z-index: 10 !important;
  white-space: normal !important;
  vertical-align: top !important;
  line-height: 1.2 !important;
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

.gray-scale {
  filter: grayscale(1);
}

.opacity-50 {
  opacity: 0.5;
}

.opacity-70 {
  opacity: 0.7;
}
</style>
