<template>
  <v-container fluid class="pa-6 analisis-dashboard">
    <!-- Top Action Bar -->
    <v-toolbar dense flat id="p" class="glass-toolbar mb-6 rounded-xl elevation-3 border-thin">
      <v-icon color="primary" class="mr-3">analytics</v-icon>
      <v-toolbar-title class="font-weight-black brand-secondary--text">
        Análisis <span class="font-weight-light grey--text">Cuadro de Inversión</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>

      <v-btn
        class="brand-btn px-4 shadow-premium mr-2"
        depressed
        @click="abrirDialogCargarSaldos"
      >
        <v-icon left>mdi-scale-balance</v-icon>
        Cargar Saldos
      </v-btn>
      
      <div class="d-flex align-center">
        <export-excel
          v-if="items.length"
          :data="items"
          worksheet="Analisis"
          :name="`Analisis_${fecha}.xls`"
          class="mr-2"
        >
          <v-btn class="brand-btn px-6 shadow-premium mr-2" depressed>
            <v-icon left>mdi-file-excel</v-icon>
            Exportar Excel
          </v-btn>
        </export-excel>

        <v-btn class="glass-btn-icon" icon @click="getReportHeaderMethodCuadroInversion" :loading="loading">
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
            label="Fecha de Corte" 
            prepend-inner-icon="event" 
            type="date" 
            filled 
            dense
            hide-details
            class="rounded-lg"
            @input="getReportHeaderMethodCuadroInversion"
          ></v-text-field>
        </v-col>
        <v-col cols="12" sm="4" md="3">
          <v-text-field 
            v-model="tipoCambio" 
            label="Tipo de Cambio" 
            prepend-inner-icon="mdi-cash-multiple" 
            type="number" 
            filled
            dense
            hide-details
            class="rounded-lg"
          ></v-text-field>
        </v-col>
      </v-row>
    </v-card>

    <!-- Main Data Table Area -->
    <v-card class="glass-card rounded-xl overflow-hidden shadow-premium border-thin">
      <v-skeleton-loader v-if="loading && items.length === 0" type="table"></v-skeleton-loader>
      
      <v-data-table 
        v-else 
        :headers="headers" 
        :items="filteredItems" 
        class="glass-table premium-table sticky-columns" 
        item-key="cuentas"
        :loading="loadingDetail" 
        :single-expand="singleExpand" 
        :expanded.sync="expanded" 
        show-expand
        fixed-header
        height="60vh"
        dense
        @item-expanded="onItemExpanded"
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
              v-if="!moneyColumns.concat(['data-table-expand']).includes(h.value)"
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

        <template v-slot:[`item.almacen`]="{ item }">
          <div class="d-flex align-center py-1">
            <v-avatar size="20" color="primary lighten-4" class="mr-2">
              <span class="text-caption font-weight-bold primary--text">{{ item.almacen ? item.almacen.charAt(0) : 'A' }}</span>
            </v-avatar>
            <span class="font-weight-bold text-truncate" style="max-width: 90px">{{ item.almacen }}</span>
          </div>
        </template>

        <template v-slot:[`item.cuentas`]="{ item }">
          <span class="font-weight-medium grey--text text--darken-2">{{ item.cuentas }}</span>
        </template>

        <template v-for="col in moneyColumns" v-slot:[`item.${col}`]="{ item }">
          <span :key="col" :class="['font-weight-medium', getColor(item[col])]">
            {{ item[col] | currency }}
          </span>
        </template>

        <template v-slot:expanded-item="{ headers, item }">
          <td :colspan="headers.length" class="pa-4 bg-faint">
            <v-card class="glass-card shadow-sm border-thin overflow-hidden rounded-xl">
              <v-toolbar flat dense class="glass-toolbar-inner" height="40">
                <v-icon small class="mr-2">mdi-details</v-icon>
                <span class="text-subtitle-2 font-weight-bold">Detalle de cuenta: {{ item.cuentas }}</span>
              </v-toolbar>
              <v-data-table 
                dense 
                :headers="headersDetails" 
                :items="itemsDetails" 
                hide-default-footer 
                disable-pagination
                disable-sort 
                class="glass-table-nested elevation-0" 
                item-key="factura" 
                :loading="loadingDetail"
              >
                <template v-for="col in moneyDetailsColumns" v-slot:[`item.${col}`]="{ item }">
                    {{ item[col] | currency }}
                </template>

                <template v-slot:[`item.totalPago`]="{ item }">
                  <v-chip color="primary" label small class="font-weight-black">
                    {{ item.totalPago | currency }}
                  </v-chip>
                </template>
              </v-data-table>
            </v-card>
          </td>
        </template>
      </v-data-table>
    </v-card>

    <!-- Diálogo: parámetros para consultas de saldos -->
    <v-dialog
      v-model="dialogCargarSaldos"
      max-width="720"
      scrollable
      content-class="dialog-cargar-saldos"
    >
      <v-card class="glass-card rounded-xl border-thin overflow-hidden dialog-saldos-card">
        <v-toolbar flat dense class="glass-toolbar rounded-t-xl mb-0">
          <v-icon color="primary" class="mr-2">mdi-database-search</v-icon>
          <v-toolbar-title class="subtitle-1 font-weight-bold">
            Datos para ejecutar consultas
          </v-toolbar-title>
          <v-spacer></v-spacer>
          <v-btn icon class="glass-btn-icon" aria-label="Cerrar" @click="dialogCargarSaldos = false">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </v-toolbar>

        <v-card-text class="pt-4 px-5 pb-2">
          <p class="text-caption grey--text text--darken-1 mb-4">
            Indique las fechas para cada consulta. Al final pulse <strong>Obtener saldos</strong>.
          </p>

          <v-row dense align="center" class="saldos-form-row mb-3">
            <v-col cols="12" sm="5" md="4" class="py-1">
              <span class="body-2 font-weight-bold saldos-label">Saldo diario</span>
            </v-col>
            <v-col cols="12" sm="7" md="8" class="py-1">
              <v-text-field
                v-model="formSaldos.saldoDiario"
                label="Fecha"
                prepend-inner-icon="event"
                type="date"
                filled
                dense
                hide-details
                class="rounded-lg"
              />
            </v-col>
          </v-row>

          <v-row dense align="center" class="saldos-form-row mb-3">
            <v-col cols="12" sm="5" md="4" class="py-1">
              <span class="body-2 font-weight-bold saldos-label">Saldo de transferencias</span>
            </v-col>
            <v-col cols="6" sm="3" md="4" class="py-1">
              <v-text-field
                v-model="formSaldos.transferenciasDesde"
                label="Fecha inicial"
                prepend-inner-icon="mdi-calendar-start"
                type="date"
                filled
                dense
                hide-details
                class="rounded-lg"
              />
            </v-col>
            <v-col cols="6" sm="4" md="4" class="py-1">
              <v-text-field
                v-model="formSaldos.transferenciasHasta"
                label="Fecha final"
                prepend-inner-icon="mdi-calendar-end"
                type="date"
                filled
                dense
                hide-details
                class="rounded-lg"
              />
            </v-col>
          </v-row>

          <v-row dense align="center" class="saldos-form-row">
            <v-col cols="12" sm="5" md="4" class="py-1">
              <span class="body-2 font-weight-bold saldos-label">Saldo de depósitos</span>
            </v-col>
            <v-col cols="6" sm="3" md="4" class="py-1">
              <v-text-field
                v-model="formSaldos.depositosDesde"
                label="Fecha inicial"
                prepend-inner-icon="mdi-calendar-start"
                type="date"
                filled
                dense
                hide-details
                class="rounded-lg"
              />
            </v-col>
            <v-col cols="6" sm="4" md="4" class="py-1">
              <v-text-field
                v-model="formSaldos.depositosHasta"
                label="Fecha final"
                prepend-inner-icon="mdi-calendar-end"
                type="date"
                filled
                dense
                hide-details
                class="rounded-lg"
              />
            </v-col>
          </v-row>
        </v-card-text>

        <v-card-actions class="px-5 pb-5 pt-2">
          <v-spacer></v-spacer>
          <v-btn text class="mr-2" @click="dialogCargarSaldos = false">Cancelar</v-btn>
          <v-btn
            class="brand-btn px-6 shadow-premium"
            depressed
            :loading="loading"
            @click="onObtenerSaldos"
          >
            <v-icon left>mdi-download-circle</v-icon>
            Obtener saldos
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="snackbar" :color="snackbarColor" :timeout="4500" top>
      {{ snackbarText }}
      <template v-slot:action="{ attrs }">
        <v-btn text v-bind="attrs" @click="snackbar = false">Cerrar</v-btn>
      </template>
    </v-snackbar>
  </v-container>
</template>

<script>
import moment from "moment";
import { mapActions } from "vuex";

export default {
  name: "AnalisisCuadroInversion",
  data: () => ({
    loading: false,
    singleExpand: true,
    loadingDetail: false,
    expanded: [],
    fecha: moment().format("YYYY-MM-DD"),
    tipoCambio: 0,
    dialogCargarSaldos: false,
    formSaldos: {
      saldoDiario: "",
      transferenciasDesde: "",
      transferenciasHasta: "",
      depositosDesde: "",
      depositosHasta: "",
    },
    /** Parámetros confirmados en el diálogo (p. ej. futuras llamadas API por rango). */
    parametrosConsultaSaldos: null,
    snackbar: false,
    snackbarText: "",
    snackbarColor: "success",
    items: [],
    filters: {
      almacen: "",
      cuentas: ""
    },
    itemsDetails: [],
    search: "",
    moneyColumns: [
      "saldoDiario", "compra", "venta", "actualChequera", "saldoInversion",
      "totalBanorte", "depositos", "depositosVenta", "transferencias",
      "depositosMes", "chequeMes", "dolaresTCDia", "dolaresTCDiario",
      "granTotal", "banorte", "banamex", "santander"
    ],
    moneyDetailsColumns: [
      "saldoVencido", "descuento1", "descuento2", "descuento3", "descuento4"
    ],
    headers: [
      { text: "Almacen", value: "almacen", width: "110px", fixed: true }, 
      { text: "Cuentas", value: "cuentas", width: "150px", fixed: true },
      { text: "Saldo Diario", value: "saldoDiario", align: "right", width: "130px" },
      { text: "Compra", value: "compra", align: "right", width: "110px" },
      { text: "Venta", value: "venta", align: "right", width: "110px" },
      { text: "Actual Chequera", value: "actualChequera", align: "right", width: "130px" },
      { text: "Saldo Inversion", value: "saldoInversion", align: "right", width: "130px" },
      { text: "Total Banorte", value: "totalBanorte", align: "right", width: "130px" },
      { text: "Depositos", value: "depositos", align: "right", width: "110px" },
      { text: "Depositos Venta", value: "depositosVenta", align: "right", width: "130px" },
      { text: "Transferencias", value: "transferencias", align: "right", width: "120px" },
      { text: "Depositos Mes", value: "depositosMes", align: "right", width: "120px" },
      { text: "Cheque Mes", value: "chequeMes", align: "right", width: "120px" },
      { text: "Dolares TC Dia", value: "dolaresTCDia", align: "right", width: "130px" },
      { text: "Dolares TC Diario", value: "dolaresTCDiario", align: "right", width: "140px" },
      { text: "Gran Total", value: "granTotal", align: "right", width: "130px" },
      { text: "Banorte", value: "banorte", align: "right", width: "110px" },
      { text: "Banamex", value: "banamex", align: "right", width: "110px" },
      { text: "Santander", value: "santander", align: "right", width: "110px" },
      { text: "", value: "data-table-expand" },
    ],
    headersDetails: [
      { text: "Factura", value: "factura" },
      { text: "Saldo Vencido", value: "saldoVencido", align: "right" },
      { text: "Desc 1", value: "descuento1", align: "right" },
      { text: "Desc 2", value: "descuento2", align: "right" },
      { text: "Desc 3", value: "descuento3", align: "right" },
      { text: "Desc 4", value: "descuento4", align: "right" },
      { text: "Total Pago", value: "totalPago", align: "right" },
    ],
  }),
  filters: {
    currency(value) {
      if (!value && value !== 0) return "-";
      return new Intl.NumberFormat("es-MX", {
        style: "currency",
        currency: "MXN",
      }).format(value);
    }
  },
  methods: {
    ...mapActions("credito", ["getReportHeaderCuadroInversion", "getReportDetailCuadroInversion"]),
    
    getColor(val) {
        return val < 0 ? 'red--text font-weight-bold' : '';
    },

    async getReportHeaderMethodCuadroInversion() {
      this.loading = true;
      let ok = false;
      try {
        this.items = await this.getReportHeaderCuadroInversion(this.fecha);
        ok = true;
      } catch (e) {
        console.error(e);
      } finally {
        this.loading = false;
      }
      return ok;
    },
    async onItemExpanded({ item, value }) {
      if (value) {
        this.loadingDetail = true;
        try {
            this.itemsDetails = await this.getReportDetailCuadroInversion(item.cuentas);
        } catch(e) {
            this.itemsDetails = [];
        } finally {
            this.loadingDetail = false;
        }
        return;
      }
      this.itemsDetails = [];
    },

    inicializarFormSaldos() {
      const d = this.fecha || moment().format("YYYY-MM-DD");
      this.formSaldos = {
        saldoDiario: d,
        transferenciasDesde: d,
        transferenciasHasta: d,
        depositosDesde: d,
        depositosHasta: d,
      };
    },

    abrirDialogCargarSaldos() {
      this.inicializarFormSaldos();
      this.dialogCargarSaldos = true;
    },

    mostrarSnackbar(text, color = "success") {
      this.snackbarText = text;
      this.snackbarColor = color;
      this.snackbar = true;
    },

    validarRangoFechas(desde, hasta, etiqueta) {
      if (!desde || !hasta) {
        this.mostrarSnackbar(`Complete las fechas de ${etiqueta}.`, "error");
        return false;
      }
      if (moment(desde).isAfter(moment(hasta))) {
        this.mostrarSnackbar(`En ${etiqueta}, la fecha inicial no puede ser posterior a la final.`, "error");
        return false;
      }
      return true;
    },

    async onObtenerSaldos() {
      const f = this.formSaldos;
      if (!f.saldoDiario) {
        this.mostrarSnackbar("Indique la fecha de saldo diario.", "error");
        return;
      }
      if (!this.validarRangoFechas(f.transferenciasDesde, f.transferenciasHasta, "saldo de transferencias")) {
        return;
      }
      if (!this.validarRangoFechas(f.depositosDesde, f.depositosHasta, "saldo de depósitos")) {
        return;
      }

      this.parametrosConsultaSaldos = { ...f };
      this.fecha = f.saldoDiario;
      this.dialogCargarSaldos = false;

      const ok = await this.getReportHeaderMethodCuadroInversion();
      if (ok) {
        this.mostrarSnackbar(
          "Parámetros guardados y tabla actualizada con la fecha de saldo diario.",
          "success"
        );
      } else {
        this.mostrarSnackbar("No se pudo cargar el informe. Revise la consola o el servidor.", "error");
      }
    },
  },
  mounted() {
    this.getReportHeaderMethodCuadroInversion();
  },
  computed: {
    filteredItems() {
      if (!Array.isArray(this.items)) return [];
      return this.items.filter(item => {
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
.analisis-dashboard {
  background: #f8fafc !important; 
  min-height: 100vh;
}
.theme--dark .analisis-dashboard { 
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

.glass-btn-icon {
  background: rgba(0,0,0,0.03) !important;
  transition: all 0.2s;
}

.glass-btn-icon:hover {
  background: rgba(0,0,0,0.08) !important;
  transform: scale(1.1);
}

.bg-faint {
  background: rgba(0,0,0,0.02);
}

.glass-toolbar-inner {
  background: rgba(248, 161, 2, 0.05) !important;
  border-bottom: 1px solid rgba(0,0,0,0.05) !important;
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

/* --- STICKY COLUMNS VUETIFY 2 --- */
.sticky-columns >>> table > thead > tr > th:nth-child(1),
.sticky-columns >>> table > tbody > tr > td:nth-child(1) {
  position: sticky !important; 
  left: 0;
  z-index: 11 !important;
  background: white !important; 
  border-right: 1px solid rgba(0,0,0,0.1) !important;
}

.theme--dark .sticky-columns >>> table > thead > tr > th:nth-child(1),
.theme--dark .sticky-columns >>> table > tbody > tr > td:nth-child(1) {
  background: #1e1e1e !important;
  border-right: 1px solid rgba(255,255,255,0.1) !important;
}

.sticky-columns >>> table > thead > tr > th:nth-child(2),
.sticky-columns >>> table > tbody > tr > td:nth-child(2) {
  position: sticky !important; 
  left: 110px;
  z-index: 11 !important;
  background: white !important;
  border-right: 2px solid rgba(0,0,0,0.1) !important;
}

.theme--dark .sticky-columns >>> table > thead > tr > th:nth-child(2),
.theme--dark .sticky-columns >>> table > tbody > tr > td:nth-child(2) {
  background: #1e1e1e !important;
  border-right: 2px solid rgba(255,255,255,0.1) !important;
}

.sticky-columns >>> table > thead > tr > th:nth-child(1),
.sticky-columns >>> table > thead > tr > th:nth-child(2) {
  background: #fdf5e6 !important;
  z-index: 12 !important;
}

.theme--dark .sticky-columns >>> table > thead > tr > th:nth-child(1),
.theme--dark .sticky-columns >>> table > thead > tr > th:nth-child(2) {
  background: #252525 !important;
}

.premium-table >>> tbody tr:hover td {
  background-color: rgba(248, 161, 2, 0.05) !important;
}

.glass-table-nested >>> thead th {
  background: transparent !important;
  font-size: 0.7rem;
  border-bottom: 1px solid rgba(0,0,0,0.05) !important;
}

.line-height-tight {
  line-height: 1.2;
}

.saldos-form-row .saldos-label {
  display: block;
  line-height: 1.35;
  color: rgba(0, 0, 0, 0.75);
}

.theme--dark .saldos-form-row .saldos-label {
  color: rgba(255, 255, 255, 0.85);
}

.dialog-saldos-card {
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.18) !important;
}
</style>

<style lang="css">
/* v-dialog content se renderiza fuera del scope del componente */
.dialog-cargar-saldos {
  border-radius: 16px !important;
}
</style>
