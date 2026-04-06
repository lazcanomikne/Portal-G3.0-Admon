<template>
  <v-container fluid class="pa-6 apartados-dashboard">
    <!-- Top Action Bar -->
    <v-toolbar dense flat id="p" class="glass-toolbar mb-6 rounded-xl elevation-3 border-thin">
      <v-icon color="primary" class="mr-3">mdi-bookmark-check</v-icon>
      <v-toolbar-title class="font-weight-black brand-secondary--text">
        Apartados <span class="font-weight-light grey--text">Financieros</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      
      <div class="d-flex align-center">
        <v-btn text small class="mr-2 grey--text text--darken-1" @click="descargarTemplate">
          <v-icon left small>mdi-download</v-icon> Plantilla
        </v-btn>
        
        <v-btn class="glass-btn-icon" icon @click="cargarDatos" :loading="loading">
          <v-icon>refresh</v-icon>
        </v-btn>
      </div>
    </v-toolbar>

    <v-row>
      <!-- Left Column: KPIs and Table -->
      <v-col cols="12" lg="8">
        <!-- Compact Metrics -->
        <v-row dense class="mb-4">
          <v-col cols="12" sm="4">
            <v-card class="metric-card-compact glass-card rounded-xl pa-3 border-left-primary shadow-premium">
              <div class="d-flex align-center justify-space-between">
                <div>
                  <div class="text-overline grey--text line-height-tight mb-0">Total Apartado</div>
                  <div class="text-h5 font-weight-black primary--text">{{ totalMonto | currency }}</div>
                </div>
                <v-avatar color="primary lighten-4" size="40">
                  <v-icon color="primary" size="24">mdi-cash-register</v-icon>
                </v-avatar>
              </div>
            </v-card>
          </v-col>
          <v-col cols="12" sm="4">
            <v-card class="metric-card-compact glass-card rounded-xl pa-3 border-left-success shadow-premium">
              <div class="d-flex align-center justify-space-between">
                <div>
                  <div class="text-overline grey--text line-height-tight mb-0">Registros</div>
                  <div class="text-h5 font-weight-black success--text">{{ items.length }}</div>
                </div>
                <v-avatar color="success lighten-4" size="40">
                  <v-icon color="success" size="24">mdi-format-list-numbered</v-icon>
                </v-avatar>
              </div>
            </v-card>
          </v-col>
          <v-col cols="12" sm="4">
            <v-card class="metric-card-compact glass-card rounded-xl pa-3 border-left-info shadow-premium">
              <div class="d-flex align-center justify-space-between">
                <div>
                  <div class="text-overline grey--text line-height-tight mb-0">Promedio</div>
                  <div class="text-h5 font-weight-black info--text">{{ promedioMonto | currency }}</div>
                </div>
                <v-avatar color="info lighten-4" size="40">
                  <v-icon color="info" size="24">mdi-calculator</v-icon>
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
            class="glass-table premium-table"
            :loading="loading"
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
                  v-if="!['acciones', 'linea'].includes(h.value)"
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

            <template v-slot:item.linea="{ index }">
              <span class="grey--text text-caption">{{ index + 1 }}</span>
            </template>

            <template v-slot:item.montoApartar="{ item }">
              <span class="font-weight-black brand-secondary--text">
                {{ item.montoApartar | currency }}
              </span>
            </template>

            <template v-slot:item.fecha="{ item }">
               <span class="font-weight-medium grey--text text--darken-2">
                 {{ item.fecha | formatDate }}
               </span>
            </template>

            <template v-slot:[`item.cuenta`]="{ item }">
              <div class="d-flex align-center">
                <v-icon x-small color="primary" class="mr-2">mdi-credit-card-outline</v-icon>
                <span class="font-weight-medium">{{ item.cuenta }}</span>
              </div>
            </template>

            <!-- Actions Slot -->
            <template v-slot:item.acciones="{ item }">
              <div class="d-flex justify-end">
                <v-btn icon small color="primary" class="mr-1" @click="editarItem(item)">
                  <v-icon small>edit</v-icon>
                </v-btn>
                <v-btn icon small color="error" @click="confirmarBorrarItem(item)">
                  <v-icon small>delete</v-icon>
                </v-btn>
              </div>
            </template>

            <template v-slot:no-data>
              <div class="text-center pa-10">
                <v-icon size="64" color="grey lighten-2">mdi-database-off</v-icon>
                <p class="text-h6 grey--text text--lighten-1 mt-4">Sin datos para esta fecha</p>
              </div>
            </template>
          </v-data-table>
        </v-card>
      </v-col>

      <!-- Right Column: Management Panel -->
      <v-col cols="12" lg="4">
        <!-- Date Config & Excel Card -->
        <v-card class="glass-card rounded-xl mb-4 pa-4 border-thin shadow-sm">
          <div class="d-flex align-center mb-4">
            <v-icon small color="primary" class="mr-2">mdi-cog</v-icon>
            <span class="text-caption font-weight-bold grey--text text-uppercase">Configuración</span>
          </div>
          
          <v-text-field 
            v-model="fecha" 
            label="Fecha de Operación" 
            prepend-inner-icon="event" 
            type="date" 
            filled 
            dense
            rounded
            class="mb-3"
            @input="cargarDatos"
          ></v-text-field>

          <v-file-input 
            v-model="selectedFile"
            label="Cargar Excel" 
            filled 
            dense 
            hide-details 
            show-size 
            rounded
            prepend-inner-icon="mdi-file-excel"
            class="mb-2"
          ></v-file-input>
          
          <v-btn 
            block 
            depressed 
            class="brand-btn mt-3" 
            :disabled="!selectedFile" 
            @click="procesarArchivoExcel"
          >
            <v-icon left>mdi-upload</v-icon> Subir Archivo
          </v-btn>
        </v-card>

        <!-- Entry Form Card -->
        <v-card class="glass-card rounded-xl pb-6 shadow-premium border-thin sticky-panel">
          <v-toolbar flat dense class="glass-toolbar-inner rounded-t-xl mb-4">
            <v-icon color="primary" class="mr-2">{{ editedIndex === -1 ? 'mdi-plus-circle' : 'mdi-pencil' }}</v-icon>
            <span class="subtitle-1 font-weight-black brand-secondary--text">
              {{ formTitle }}
            </span>
            <v-spacer></v-spacer>
            <v-btn v-if="editedIndex > -1" icon small color="primary" @click="resetFormulario">
               <v-icon>mdi-plus</v-icon>
            </v-btn>
          </v-toolbar>

          <v-card-text class="px-6">
            <v-form ref="form" v-model="validForm">
              <v-autocomplete
                v-model="editedItem.cuenta"
                label="Cuenta *"
                :items="listaCuentas" 
                item-text="display"
                item-value="cuenta"
                filled
                dense
                rounded
                class="mb-2"
                required
                :rules="[v => !!v || 'Requerido']"
                return-object
              ></v-autocomplete>

              <v-text-field
                v-model.number="editedItem.montoApartar"
                label="Monto *"
                prefix="$"
                type="number"
                filled
                dense
                rounded
                class="mb-2"
                required
                :rules="[v => !!v || 'Requerido']"
              ></v-text-field>

              <v-text-field
                v-model="editedItem.fecha"
                label="Fecha *"
                type="date"
                filled
                dense
                rounded
                required
                :rules="[v => !!v || 'Requerido']"
              ></v-text-field>
            </v-form>
          </v-card-text>

          <v-card-actions class="px-6">
            <v-btn 
              color="primary" 
              block 
              @click="guardar" 
              :disabled="!validForm" 
              :loading="overlay" 
              class="brand-btn py-6 shadow-premium"
            >
              <v-icon left>{{ editedIndex === -1 ? 'mdi-content-save' : 'mdi-check' }}</v-icon>
              {{ editedIndex === -1 ? 'Crear Apartado' : 'Actualizar' }}
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <!-- Generic Modal for Deletion -->
    <v-dialog v-model="dialogDelete" max-width="400px">
      <v-card class="rounded-xl pa-4 text-center glass-card">
        <v-avatar color="error lighten-4" size="70" class="mb-4">
          <v-icon color="error" size="40">mdi-alert-circle</v-icon>
        </v-avatar>
        <v-card-title class="headline justify-center font-weight-black error--text">¿Eliminar?</v-card-title>
        <v-card-text>
          Cuenta: <strong>{{ itemToDelete.cuenta }}</strong><br>
          Esta acción removerá el apartado permanentemente.
        </v-card-text>
        <v-card-actions class="justify-center pt-4">
          <v-btn text @click="dialogDelete = false">Cancelar</v-btn>
          <v-btn color="error" class="rounded-lg px-8 ml-2 shadow-sm" @click="borrarItemConfirmado">Confirmar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Overlay & Notifications -->
    <v-overlay :value="overlay" z-index="300" opacity="0.7">
      <v-progress-circular indeterminate size="64" color="primary"></v-progress-circular>
    </v-overlay>

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" rounded="pill" class="mb-4">
      <div class="d-flex align-center font-weight-bold">
        <v-icon dark class="mr-2">{{ snackbar.color === 'success' ? 'mdi-check-circle' : 'mdi-alert' }}</v-icon>
        {{ snackbar.text }}
      </div>
    </v-snackbar>
  </v-container>
</template>

<script>
import moment from "moment";
import { mapActions, mapState } from "vuex";
import xlsx from "xlsx";

export default {
  name: "Apartados",
  data: () => ({
    fecha: moment().format("YYYY-MM-DD"),
    items: [],
    loading: false,
    overlay: false,
    selectedFile: null,
    listaCuentas: [], 
    dialogDelete: false,
    validForm: false,
    editedIndex: -1,
    filters: {
      cuenta: "",
      montoApartar: "",
      fecha: ""
    },
    editedItem: { id: 0, cuenta: null, montoApartar: 0, fecha: moment().format("YYYY-MM-DD") },
    defaultItem: { id: 0, cuenta: null, montoApartar: 0, fecha: moment().format("YYYY-MM-DD") },
    itemToDelete: {},
    headers: [
      { text: "#", value: "linea", width: "50px", align: "center", sortable: false },
      { text: "Cuenta", value: "cuenta", sortable: true },
      { text: "Monto Apartar", value: "montoApartar", align: "end", sortable: true, width: "160px" },
      { text: "Fecha", value: "fecha", align: "center", sortable: true, width: "130px" },
      { text: "Acciones", value: "acciones", sortable: false, width: "100px", align: "center" },
    ],
    snackbar: { show: false, text: '', color: 'success' }
  }),
  
  mixins: [], // No mixin used here initially, but keeping structure

  filters: {
    currency(value) {
      if (!value && value !== 0) return "$0.00";
      return new Intl.NumberFormat("es-MX", { style: "currency", currency: "MXN" }).format(value);
    },
    formatDate(value) {
      if (!value) return "";
      return moment(value).format("DD/MM/YYYY");
    }
  },

  computed: {
    ...mapState("login", ["userName"]),
    formTitle() { return this.editedIndex === -1 ? 'Nuevo Apartado' : 'Editar Apartado'; },
    
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
    },

    totalMonto() { return this.items.reduce((acc, curr) => acc + (parseFloat(curr.montoApartar)||0), 0); },
    promedioMonto() { return this.items.length > 0 ? this.totalMonto / this.items.length : 0; }
  },

  mounted() {
    this.cargarDatos();
    this.cargarCatalogoCuentas();
    this.initFilters();
  },

  methods: {
    ...mapActions("inversion", [
      "getApartados", "postApartado", "updateApartado", "deleteApartado", "postApartadosExcel", "getCuentas"
    ]),

    initFilters() {
      const f = {};
      this.headers.forEach(h => { if(!['acciones', 'linea'].includes(h.value)) f[h.value] = ""; });
      this.filters = f;
    },

    cargarCatalogoCuentas() {
      this.getCuentas().then(res => {
         this.listaCuentas = res.map(x => ({
           almacen: x.almacen,
           cuenta: x.cuenta,
           display: `${x.almacen} - ${x.cuenta}`
         }));
      });
    },

    async cargarDatos() {
      this.loading = true;
      try {
        const res = await this.getApartados(this.fecha);
        // Normalizar ID
        this.items = (res || []).map(i => ({
           ...i,
           id: i.id || i.ID,
           cuenta: i.cuenta || i.Cuenta,
           montoApartar: i.montoApartar || i.MontoApartar,
           fecha: i.fecha || i.Fecha
        }));
      } catch (err) {
        console.error(err);
        this.mostrarSnack("Error cargando apartados", "error");
      } finally {
        this.loading = false;
      }
    },

    descargarTemplate() {
      const ws = xlsx.utils.aoa_to_sheet([["Cuenta", "Monto"]]);
      const wb = xlsx.utils.book_new();
      xlsx.utils.book_append_sheet(wb, ws, "Template");
      xlsx.writeFile(wb, "Template_Apartados.xlsx");
    },

    resetFormulario() {
      this.editedItem = Object.assign({}, this.defaultItem);
      this.editedItem.fecha = this.fecha; 
      this.editedIndex = -1;
      this.$refs.form?.resetValidation();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    },

    editarItem(item) {
      this.editedIndex = this.items.indexOf(item);
      this.editedItem = Object.assign({}, item);
      const match = this.listaCuentas.find(c => c.cuenta === item.cuenta);
      if (match) {
         this.editedItem.cuenta = match;
      } else {
         this.editedItem.cuenta = { cuenta: item.cuenta, display: item.cuenta }; 
      }
      this.editedItem.fecha = moment(item.fecha).format("YYYY-MM-DD");
      window.scrollTo({ top: 0, behavior: 'smooth' });
    },

    async guardar() {
      if (!this.$refs.form.validate()) return;
      this.overlay = true;
      try {
        const cuentaValue = this.editedItem.cuenta.cuenta || this.editedItem.cuenta; 
        const payload = { 
            Id: this.editedItem.id, // PascalCase for C#
            Cuenta: cuentaValue,
            MontoApartar: parseFloat(this.editedItem.montoApartar),
            Fecha: this.editedItem.fecha
        }; 

        if (this.editedIndex > -1) {
          await this.updateApartado(payload);
          this.mostrarSnack("Apartado actualizado", "success");
        } else {
          await this.postApartado(payload);
          this.mostrarSnack("Apartado creado", "success");
        }
        await this.cargarDatos();
        this.resetFormulario();
      } catch (err) {
        this.mostrarSnack("Error al guardar", "error");
        console.error(err);
      } finally {
        this.overlay = false;
      }
    },

    confirmarBorrarItem(item) {
      this.itemToDelete = item;
      this.dialogDelete = true;
    },

    async borrarItemConfirmado() {
      this.overlay = true;
      try {
        await this.deleteApartado(this.itemToDelete.id);
        this.mostrarSnack("Registro eliminado", "success");
        await this.cargarDatos();
        this.dialogDelete = false;
      } catch (err) {
        this.mostrarSnack("Error al eliminar", "error");
        console.error(err);
      } finally {
        this.overlay = false;
      }
    },

    procesarArchivoExcel() {
      if (!this.selectedFile) return;
      this.overlay = true;
      const fileReader = new FileReader();
      fileReader.onload = (ev) => {
        try {
          const data = ev.target.result;
          const workbook = xlsx.read(data, { type: "binary" });
          const wsname = workbook.SheetNames[0];
          const ws = xlsx.utils.sheet_to_json(workbook.Sheets[wsname]);
          const detalle = ws.map(row => ({
            Cuenta: String(row["Cuenta"] || row["cuenta"]),
            MontoApartar: parseFloat(row["Monto"] || row["monto"]),
            Fecha: this.fecha 
          }));
          if (detalle.length === 0) throw new Error("Archivo vacío");
          this.postApartadosExcel(detalle).then(() => {
            this.mostrarSnack("Carga masiva exitosa", "success");
            this.selectedFile = null;
            this.cargarDatos();
          });
        } catch (e) {
          this.mostrarSnack("Error leyendo Excel", "error");
        } finally {
          this.overlay = false;
        }
      };
      fileReader.readAsBinaryString(this.selectedFile);
    },

    mostrarSnack(text, color) { this.snackbar = { show: true, text, color }; }
  }
};
</script>

<style scoped>
.apartados-dashboard { 
  background: #f8fafc !important; /* Soft light background for light mode */
  min-height: 100vh; 
}
.theme--dark .apartados-dashboard { 
  background: #0f172a !important; /* Proper dark mode background */
}
.glass-card {
  background: rgba(255, 255, 255, 0.7) !important;
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.3) !important;
}
.theme--dark .glass-card { background: rgba(30,30,30, 0.6) !important; }

.shadow-premium { box-shadow: 0 10px 30px -10px rgba(0,0,0,0.1) !important; }
.border-thin { border: 1px solid rgba(0,0,0,0.05) !important; }
.border-left-primary { border-left: 4px solid var(--v-primary-base) !important; }
.border-left-success { border-left: 4px solid #4CAF50 !important; }
.border-left-info { border-left: 4px solid #2196F3 !important; }

.brand-btn {
  background: linear-gradient(135deg, #f8a102 0%, #ffc107 100%) !important;
  color: white !important;
  font-weight: bold !important;
  border-radius: 12px !important;
  text-transform: none !important;
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

.compact-filter-input >>> .v-input__slot { padding: 0 4px !important; border-radius: 4px !important; min-height: 20px !important; }
.sticky-panel { position: sticky; top: 88px; }
.bg-faint { background: rgba(0,0,0,0.01); }
.line-height-tight { line-height: 1.1; }
</style>
