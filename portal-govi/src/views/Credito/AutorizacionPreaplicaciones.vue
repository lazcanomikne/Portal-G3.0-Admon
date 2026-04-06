<template>
  <v-container fluid class="pa-6 preapp-dashboard">
    <!-- Top Action Bar -->
    <v-toolbar dense flat id="p" class="glass-toolbar mb-6 rounded-xl elevation-3 border-thin">
      <v-icon color="primary" class="mr-3">bolt</v-icon>
      <v-toolbar-title class="font-weight-black brand-secondary--text">
        Ejecutar <span class="font-weight-light grey--text">Preaplicaciones</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      
      <div class="d-flex align-center">
        <v-btn icon color="primary" @click="getAutorizacionPreaplicacionesMethod" class="mr-4 glass-btn-icon shadow-sm">
          <v-icon>refresh</v-icon>
        </v-btn>
        <v-btn class="brand-btn px-6 shadow-premium" depressed :disabled="!selected.length"
          @click="updateAutorizacionPreaplicacionesMethod">
          <v-icon left>check_circle</v-icon>
          Aplicar Operaciones ({{ selected.length }})
        </v-btn>
      </div>
    </v-toolbar>

    <!-- Metrics Row -->
    <v-row class="mb-6">
      <v-col cols="12" sm="4">
        <v-card class="metric-card glass-card rounded-xl pa-4 border-left-info shadow-premium">
          <div class="d-flex align-center">
            <v-avatar color="info lighten-4" size="52" class="mr-4">
              <v-icon color="info">pending_actions</v-icon>
            </v-avatar>
            <div>
              <div class="text-overline font-weight-black grey--text line-height-tight mb-1">Pendientes</div>
              <div class="text-h4 font-weight-black brand-secondary--text">{{ items.length }}</div>
            </div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="12" sm="4">
        <v-card class="metric-card glass-card rounded-xl pa-4 border-left-success shadow-premium">
          <div class="d-flex align-center">
            <v-avatar color="success lighten-4" size="52" class="mr-4">
              <v-icon color="success">payments</v-icon>
            </v-avatar>
            <div>
              <div class="text-overline font-weight-black grey--text line-height-tight mb-1">Monto Total</div>
              <div class="text-h4 font-weight-black success--text">{{ totalItemsAmount | currency }}</div>
            </div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="12" sm="4">
        <v-card class="metric-card glass-card rounded-xl pa-4 border-left-primary shadow-premium">
          <div class="d-flex align-center">
            <v-avatar color="primary lighten-4" size="52" class="mr-4">
              <v-icon color="primary">fact_check</v-icon>
            </v-avatar>
            <div>
              <div class="text-overline font-weight-black grey--text line-height-tight mb-1">Seleccionado</div>
              <div class="text-h4 font-weight-black brand-secondary--text">{{ totalSelectedAmount | currency }}</div>
            </div>
          </div>
        </v-card>
      </v-col>
    </v-row>


    <!-- Main Data Area -->
    <v-card class="glass-card rounded-xl overflow-hidden shadow-premium border-thin">
      <v-skeleton-loader type="table" v-if="loading"></v-skeleton-loader>
      <v-data-table 
        v-else
        class="glass-table premium-table" 
        item-key="folioPago" 
        v-model="selected" 
        :headers="headers" 
        :items="filteredItems" 
        :loading="loadingDetail" 
        :single-expand="singleExpand" 
        :expanded.sync="expanded"
        fixed-header 
        hide-default-footer 
        disable-pagination 
        show-expand 
        show-select 
        height="60vh"
        @item-expanded="onItemExpanded"
      >
        <!-- Custom Headers for Filtering -->
        <template v-for="h in headers" v-slot:[`header.${h.value}`]="{ header }">
          <div :key="h.value" class="d-flex flex-column py-2" v-if="!['data-table-expand', 'data-table-select'].includes(h.value)">
            <span class="mb-1">{{ header.text }}</span>
            <v-text-field
              v-if="!['totalAPagar', 'monto', 'actions'].includes(h.value)"
              v-model="filters[h.value]"
              dense
              hide-details
              filled
              flat
              solo
              background-color="rgba(0,0,0,0.03)"
              class="compact-filter-input"
              placeholder="Filtrar..."
              clearable
            ></v-text-field>
          </div>
          <span v-else :key="h.value">{{ header.text }}</span>
        </template>
        <template v-slot:top>
          <v-dialog v-model="dialogDelete" max-width="500px">
            <v-card class="rounded-xl pa-4">
              <v-card-title class="headline d-flex justify-center flex-column pb-2">
                <v-avatar color="error lighten-5" size="64" class="mb-4">
                  <v-icon color="error" large>warning_amber</v-icon>
                </v-avatar>
                ¿Confirmar eliminación?
              </v-card-title>
              <v-card-text class="text-center">
                Esta acción eliminará la preaplicación de forma permanente.
              </v-card-text>
              <v-card-actions class="justify-center pt-4">
                <v-btn color="grey darken-1" text class="rounded-lg px-6" @click="closeDelete">Cancelar</v-btn>
                <v-btn color="error" class="rounded-lg px-6 shadow-sm" depressed @click="deleteItemConfirm">Sí, eliminar</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
        </template>

        <!-- Custom Item Slots -->
        <template v-slot:[`item.totalAPagar`]="{ item }">
          <span class="font-weight-black brand-secondary--text"> {{ item.totalAPagar | currency }} </span>
        </template>

        <template v-slot:[`item.monto`]="{ item }">
          <span class="font-weight-medium success--text"> {{ item.monto | currency }} </span>
        </template>

        <template v-slot:[`item.estatus`]="{ item }">
          <v-chip 
            small 
            :color="item.estatus === 'Pendiente' ? 'amber lighten-4' : 'info lighten-4'" 
            :text-color="item.estatus === 'Pendiente' ? 'amber darken-4' : 'info darken-4'"
            class="font-weight-bold"
          >
            {{ item.estatus }}
          </v-chip>
        </template>

        <template v-slot:[`item.manual`]="{ item }">
          <v-chip x-small outlined :color="item.manual ? 'purple' : 'blue'" class="font-weight-bold">
            {{ item.manual ? 'MANUAL' : 'AUTO' }}
          </v-chip>
        </template>

        <template v-slot:[`item.cliente`]="{ item }">
          <div class="d-flex flex-column py-2">
            <span class="text-subtitle-2 font-weight-bold line-height-tight">{{ item.cliente }}</span>
            <small class="grey--text font-weight-medium">{{ item.cardCode }}</small>
          </div>
        </template>

        <template v-slot:[`item.fecha`]="{ item }">
          <span class="text-caption font-weight-medium grey--text text--darken-1"> {{ item.fecha | textcrop2(10) }} </span>
        </template>

        <template v-slot:[`item.actions`]="{ item }">
          <div class="d-flex align-center">
            <v-tooltip bottom>
              <template v-slot:activator="{ on, attrs }">
                <v-btn icon small v-on="on" v-bind="attrs" color="primary" class="mr-1 glass-btn-icon" @click="irAPago(item.folioPago)">
                  <v-icon small>edit</v-icon>
                </v-btn>
              </template>
              <span>Editar Preaplicación</span>
            </v-tooltip>
            
            <v-tooltip bottom>
              <template v-slot:activator="{ on, attrs }">
                <v-btn icon small v-on="on" v-bind="attrs" color="error" class="glass-btn-icon" @click="deleteItem(item)">
                  <v-icon small>delete</v-icon>
                </v-btn>
              </template>
              <span>Eliminar</span>
            </v-tooltip>
          </div>
        </template>

        <!-- Expanded sub-table -->
        <template v-slot:expanded-item="{ headers, item }">
          <td :colspan="headers.length" class="pa-4 bg-faint">
            <v-card flat class="glass-card rounded-xl border-thin">
              <v-toolbar flat dense color="transparent">
                <v-icon small class="mr-2">list</v-icon>
                <span class="text-overline font-weight-bold grey--text">Desglose de Facturas</span>
              </v-toolbar>
              <v-data-table 
                dense 
                :headers="headersDetails" 
                :items="itemsDetails" 
                hide-default-footer
                disable-pagination 
                disable-sort 
                class="glass-table-nested" 
                item-key="id" 
                :loading="loadingDetail"
              >
                <template v-slot:[`item.factura`]="{ item }">
                   <span class="font-weight-bold color-primary-dark">{{ item.factura }}</span>
                </template>
                <template v-slot:[`item.saldoVencido`]="{ item }">
                  <span class="font-weight-medium"> {{ item.saldoVencido | currency }} </span>
                </template>
                <template v-slot:[`item.totalPago`]="{ item }">
                  <v-chip x-small color="success lighten-4" text-color="success darken-3" class="font-weight-bold">
                    {{ item.totalPago | currency }}
                  </v-chip>
                </template>
                <!-- Smaller currency for discounts -->
                <template v-slot:[`item.descuento1`]="{ item }"><small class="grey--text">{{ item.descuento1 | currency }}</small></template>
                <template v-slot:[`item.descuento2`]="{ item }"><small class="grey--text">{{ item.descuento2 | currency }}</small></template>
                <template v-slot:[`item.descuento3`]="{ item }"><small class="grey--text">{{ item.descuento3 | currency }}</small></template>
                <template v-slot:[`item.descuento4`]="{ item }"><small class="grey--text">{{ item.descuento4 | currency }}</small></template>
              </v-data-table>
            </v-card>
          </td>
        </template>
      </v-data-table>
    </v-card>
  </v-container>
</template>
<script>
import { mapActions } from "vuex";
export default {
  name: "AutorizacionPreaplicaciones",
  data: () => ({
    loading: false,
    singleExpand: false,
    loadingDetail: false,
    expanded: [],
    selected: [],
    items: [],
    itemsDetails: [],
    search: "",
    filters: {
      cliente: "",
      folioPago: "",
      sucursal: "",
      estatus: "",
      folioSAP: "",
      fecha: "",
      manual: ""
    },
    dialogDelete: false,
    headersDetails: [
      { text: "Factura", value: "factura" },
      { text: "Saldo Vencido", value: "saldoVencido", align: "right" },
      { text: "Desc 1", value: "descuento1", align: "right" },
      { text: "Desc 2", value: "descuento2", align: "right" },
      { text: "Desc 3", value: "descuento3", align: "right" },
      { text: "Desc 4", value: "descuento4", align: "right" },
      { text: "Total Pago", value: "totalPago", align: "right" },
    ],
    headers: [
      { text: "Cliente", value: "cliente" },
      { text: "Folio", value: "folioPago" },
      { text: "Sucursal", value: "sucursal" },
      { text: "Estado", value: "estatus" },
      { text: "Folio SAP", value: "folioSAP" },
      { text: "Fecha", value: "fecha" },
      { text: "Total a Pagar", value: "totalAPagar", align: "right" },
      { text: "Monto Banco", value: "monto", align: "right" },
      { text: "Manual", value: "manual" },
      { text: "", value: "actions" },
      { text: "", value: "data-table-expand" },
    ],
    editedIndex: -1,
  }),
  watch: {
    dialog (val) {
      val || this.close();
    },
    dialogDelete (val) {
      val || this.closeDelete();
    },
  },
  mounted () {
    this.getAutorizacionPreaplicacionesMethod()
  },
  methods: {
    ...mapActions("credito", [
      "getAutorizacionPreaplicaciones",
      "updateAutorizacionPreaplicaciones",
      "getReportDetail",
      "getPagoByFolio",
      "deletePagoByFolio"
    ]),
    async getAutorizacionPreaplicacionesMethod () {
      this.loading = true;
      try {
        const response = await this.getAutorizacionPreaplicaciones();
        this.items = Array.isArray(response) ? response : [];
      } catch (error) {
        console.error("Error loading preapplications:", error);
        this.items = [];
      } finally {
        this.loading = false;
      }
    },
    async updateAutorizacionPreaplicacionesMethod () {
      this.loading = true;
      try {
        await Promise.all(this.selected.map(item => this.updateAutorizacionPreaplicaciones(item.folioPago)));
        this.selected = []; // Limpiar selección tras aplicar
        await this.getAutorizacionPreaplicacionesMethod();
      } catch (error) {
        console.error("Error updating preapplications:", error);
      } finally {
        this.loading = false;
      }
    },
    async onItemExpanded ({ item, value }) {
      if (value) {
        this.loadingDetail = true;
        this.itemsDetails = await this.getReportDetail(item.folioPago);
        this.loadingDetail = false;
        return;
      }
      this.itemsDetails = [];
    },
    async borrarPagoPorFolio (folioPago) {
      try {
        this.loading = true;
        await this.deletePagoByFolio(folioPago);
        alert('Registro borrado')
        this.loading = false;
      } catch (e) {
        this.loading = false;
      }
      this.getAutorizacionPreaplicacionesMethod()
    },
    irAPago (folioPago) {
      this.$store.commit("credito/SET_PAGO", null); // Limpiar pago anterior
      this.$store.commit("credito/SET_ERROR", null);
      this.$store.commit("credito/SET_PAGO", { folioPago }); // Setear folioPago temporalmente
      this.$router.push({ name: "Pagos" });
    },
    deleteItem (item) {
      this.editedIndex = this.items.indexOf(item);
      this.dialogDelete = true;
    },
    deleteItemConfirm () {
      const { folioPago } = this.items[this.editedIndex]
      this.items.splice(this.editedIndex, 1);
      this.borrarPagoPorFolio(folioPago)
      this.closeDelete();
    },
    closeDelete () {
      this.dialogDelete = false;
      this.$nextTick(() => {
        this.editedIndex = -1;
      });
    },
  },
  computed: {
    totalItemsAmount() {
      if (!Array.isArray(this.filteredItems)) return 0;
      return this.filteredItems.reduce((acc, item) => acc + (parseFloat(item.totalAPagar) || 0), 0);
    },
    totalSelectedAmount() {
      if (!Array.isArray(this.selected)) return 0;
      return this.selected.reduce((acc, item) => acc + (parseFloat(item.totalAPagar) || 0), 0);
    },
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
.preapp-dashboard {
  background: #f8fafc !important; 
  min-height: 100vh;
}
.theme--dark .preapp-dashboard { 
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

.premium-search >>> .v-input__control {
  min-height: 48px !important;
}

.premium-table >>> .v-data-table__wrapper table {
  font-size: 0.875rem;
}

.premium-table >>> thead th {
  background: #fdf5e6 !important;
  color: var(--v-secondary-base) !important;
  font-weight: 800 !important;
  text-transform: uppercase;
  font-size: 0.75rem;
  letter-spacing: 1px;
  z-index: 10 !important;
}

.theme--dark .premium-table >>> thead th {
  background: #252525 !important;
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

.glass-table-nested >>> thead th {
  background: transparent !important;
  font-size: 0.7rem;
  border-bottom: 1px solid rgba(0,0,0,0.05) !important;
}

.color-primary-dark {
  color: var(--v-primary-darken2);
}

.line-height-tight {
  line-height: 1.2;
}

.v-data-table >>> tbody tr:hover {
  background-color: rgba(248, 161, 2, 0.03) !important;
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

.premium-table >>> thead th {
  height: auto !important;
  padding-top: 4px !important;
  padding-bottom: 4px !important;
  vertical-align: top !important;
}
</style>
