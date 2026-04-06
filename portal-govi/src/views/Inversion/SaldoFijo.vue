<template>
  <v-container fluid class="pa-6 fixo-dashboard">
    <!-- Top Action Bar -->
    <v-toolbar dense flat id="p" class="glass-toolbar mb-6 rounded-xl elevation-3 border-thin">
      <v-icon color="primary" class="mr-3">lock</v-icon>
      <v-toolbar-title class="font-weight-black brand-secondary--text">
        Saldo <span class="font-weight-light grey--text">Fijo</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      
      <div class="d-flex align-center">
        <export-excel
          v-if="registros.length"
          :data="filteredItems"
          worksheet="SaldosFijos"
          :name="`Saldos_Fijos_${new Date().toISOString().substr(0,10)}.xls`"
          class="mr-2"
        >
          <v-btn class="brand-btn px-6 shadow-premium" depressed>
            <v-icon left>mdi-microsoft-excel</v-icon>
            Exportar
          </v-btn>
        </export-excel>

        <v-btn class="glass-btn-icon ml-2" icon @click="cargarRegistros" :loading="cargando">
          <v-icon>refresh</v-icon>
        </v-btn>
      </div>
    </v-toolbar>

    <v-row>
      <!-- Left Column: Metrics and Table -->
      <v-col cols="12" lg="8">
        <!-- Compact Metrics Card row -->
        <v-row dense class="mb-4">
          <v-col cols="12" sm="4">
            <v-card class="metric-card-compact glass-card rounded-xl pa-3 border-left-success shadow-premium">
              <div class="d-flex align-center justify-space-between">
                <div>
                  <div class="text-overline grey--text line-height-tight mb-0">Con Saldo</div>
                  <div class="text-h5 font-weight-black success--text">{{ kpiWithBalance }}</div>
                </div>
                <v-avatar color="success lighten-4" size="40">
                  <v-icon color="success" size="24">mdi-check-circle-outline</v-icon>
                </v-avatar>
              </div>
            </v-card>
          </v-col>
          <v-col cols="12" sm="4">
            <v-card class="metric-card-compact glass-card rounded-xl pa-3 border-left-warning shadow-premium">
              <div class="d-flex align-center justify-space-between">
                <div>
                  <div class="text-overline grey--text line-height-tight mb-0">Sin Saldo</div>
                  <div class="text-h5 font-weight-black warning--text">{{ kpiWithoutBalance }}</div>
                </div>
                <v-avatar color="warning lighten-4" size="40">
                  <v-icon color="warning" size="24">mdi-alert-circle-outline</v-icon>
                </v-avatar>
              </div>
            </v-card>
          </v-col>
          <v-col cols="12" sm="4">
            <v-card class="metric-card-compact glass-card rounded-xl pa-3 border-left-primary shadow-premium">
              <div class="d-flex align-center justify-space-between">
                <div>
                  <div class="text-overline grey--text line-height-tight mb-0">Total</div>
                  <div class="text-h5 font-weight-black brand-secondary--text">{{ registros.length }}</div>
                </div>
                <v-avatar color="primary lighten-4" size="40">
                  <v-icon color="primary" size="24">mdi-bank</v-icon>
                </v-avatar>
              </div>
            </v-card>
          </v-col>
        </v-row>

        <!-- Main Data Table -->
        <v-card class="glass-card rounded-xl overflow-hidden shadow-premium border-thin">
          <v-data-table
            :headers="columnas"
            :items="filteredItems"
            class="glass-table premium-table"
            :loading="cargando"
            dense
            fixed-header
            height="calc(100vh - 320px)"
            :items-per-page="50"
            :footer-props="{
              'items-per-page-options': [50, 100, -1]
            }"
          >
            <!-- Custom Headers for Filtering -->
            <template v-for="h in columnas" v-slot:[`header.${h.value}`]="{ header }">
              <div :key="h.value" class="d-flex flex-column py-2">
                <span class="mb-1">{{ header.text }}</span>
                <v-text-field
                  v-if="!['acciones', 'id'].includes(h.value)"
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
                <v-icon size="64" color="grey lighten-2">mdi-database-off</v-icon>
                <p class="text-h6 grey--text text--lighten-1 mt-4">Vacio</p>
              </div>
            </template>
            
            <template v-slot:[`item.saldoFijo`]="{ item }">
              <span class="font-weight-black" :class="(parseFloat(item.saldoFijo)||0) > 0 ? 'green--text text--darken-2' : 'grey--text'">
                {{ item.saldoFijo | currency }}
              </span>
            </template>

            <template v-slot:[`item.titular`]="{ item }">
              <div class="d-flex align-center">
                <v-avatar size="24" color="primary lighten-4" class="mr-2">
                  <span class="text-caption font-weight-bold primary--text">{{ (item.titular || 'A').charAt(0) }}</span>
                </v-avatar>
                <span class="font-weight-medium text-truncate" style="max-width: 200px">{{ item.titular }}</span>
              </div>
            </template>

            <!-- Actions Slot -->
            <template v-slot:[`item.acciones`]="{ item }">
              <div class="d-flex justify-end">
                <v-btn icon small color="primary" class="mr-1" @click="editarRegistro(item)">
                  <v-icon small>edit</v-icon>
                </v-btn>
                <v-btn icon small color="info" class="mr-1" @click="verDetalle(item)">
                  <v-icon small>preview</v-icon>
                </v-btn>
                <v-btn icon small color="error" @click="confirmarEliminacion(item)">
                  <v-icon small>delete</v-icon>
                </v-btn>
              </div>
            </template>
          </v-data-table>
        </v-card>
      </v-col>

      <!-- Right Column: Management Panel -->
      <v-col cols="12" lg="4">
        <v-card class="glass-card rounded-xl pb-6 shadow-premium border-thin sticky-panel">
          <v-toolbar flat dense class="glass-toolbar-inner rounded-t-xl mb-4">
            <v-icon color="primary" class="mr-2">{{ esEdicion ? 'edit' : 'add_circle' }}</v-icon>
            <span class="subtitle-1 font-weight-black brand-secondary--text">
              {{ esEdicion ? 'Editar Registro' : 'Nuevo Registro' }}
            </span>
            <v-spacer></v-spacer>
            <v-btn v-if="esEdicion" icon small color="primary" @click="resetFormulario">
               <v-icon>add</v-icon>
            </v-btn>
          </v-toolbar>

          <v-card-text class="px-6">
            <v-form ref="formulario" v-model="formularioValido">
              <v-text-field 
                v-model="registroEditando.titular" 
                label="Titular *" 
                filled
                dense
                rounded
                class="mb-2"
                :rules="reglas.titular" 
                required
              ></v-text-field>

              <v-text-field 
                v-model="registroEditando.cuenta" 
                label="Número de Cuenta *" 
                filled
                dense
                rounded
                class="mb-2"
                :rules="reglas.cuenta" 
                required
              ></v-text-field>

              <v-text-field 
                v-model="registroEditando.saldoFijo" 
                label="Saldo Fijo *" 
                type="number"
                step="0.01" 
                filled
                dense
                rounded
                prefix="$"
                class="mb-4"
                :rules="reglas.saldoFijo" 
                required
              ></v-text-field>
            </v-form>

            <v-alert v-if="esEdicion" dense text type="info" class="rounded-lg mb-4 text-caption">
              Editando registro <span class="font-weight-black">#{{ registroEditando.id }}</span>. Pulse el botón "+" para crear uno nuevo.
            </v-alert>
          </v-card-text>

          <v-card-actions class="px-6">
            <v-btn 
              color="primary" 
              block 
              @click="guardarRegistro" 
              :disabled="!formularioValido" 
              :loading="guardando" 
              class="brand-btn py-6 shadow-premium"
            >
              <v-icon left>{{ esEdicion ? 'save' : 'add' }}</v-icon>
              {{ esEdicion ? 'Actualizar Registro' : 'Guardar Nuevo' }}
            </v-btn>
          </v-card-actions>
        </v-card>

        <!-- Information Card -->
        <v-card class="glass-card rounded-xl mt-4 pa-4 border-thin shadow-sm bg-faint">
          <div class="d-flex align-center mb-2">
            <v-icon small color="primary" class="mr-2">info</v-icon>
            <span class="text-caption font-weight-bold grey--text text-uppercase">Información</span>
          </div>
          <p class="text-caption grey--text mb-0">
            Los cambios realizados en Saldo Fijo son permanentes y afectan los cálculos de inversión global. 
            Asegúrese de validar los números de cuenta antes de guardar.
          </p>
        </v-card>
      </v-col>
    </v-row>

    <!-- Modal Dialogs (Detail & Delete) -->
    <v-dialog v-model="dialogoDetalle" max-width="500px">
      <v-card class="rounded-xl glass-card overflow-hidden">
        <v-toolbar flat dense class="glass-toolbar-inner">
          <v-icon color="info" class="mr-2">info_outline</v-icon>
          <span class="text-h6 font-weight-black brand-secondary--text text-truncate">Detalle</span>
          <v-spacer></v-spacer>
          <v-btn icon small @click="dialogoDetalle = false"><v-icon>close</v-icon></v-btn>
        </v-toolbar>
        <v-card-text class="pt-6">
          <div class="detail-item glass-card rounded-lg pa-4 mb-3 border-thin">
             <div class="text-overline grey--text">Titular</div>
             <div class="text-h6 font-weight-bold">{{ registroSeleccionado.titular }}</div>
          </div>
          <div class="detail-item glass-card rounded-lg pa-4 mb-3 border-thin">
             <div class="text-overline grey--text">Cuenta</div>
             <div class="text-h6 font-weight-bold">{{ registroSeleccionado.cuenta }}</div>
          </div>
          <div class="detail-item glass-card rounded-lg pa-4 mb-3 border-thin border-left-success">
             <div class="text-overline grey--text">Saldo Fijo</div>
             <div class="text-h4 font-weight-black success--text">{{ registroSeleccionado.saldoFijo | currency }}</div>
          </div>
        </v-card-text>
      </v-card>
    </v-dialog>

    <v-dialog v-model="dialogoEliminacion" max-width="450px">
      <v-card class="rounded-xl pa-4 text-center glass-card">
        <v-avatar color="error lighten-4" size="70" class="mb-4">
          <v-icon color="error" size="40">mdi-alert-octagon</v-icon>
        </v-avatar>
        <v-card-title class="headline justify-center font-weight-black error--text">Eliminar</v-card-title>
        <v-card-text>
          ¿Confirma eliminar a <strong>{{ registroAEliminar.titular }}</strong>?
        </v-card-text>
        <v-card-actions class="justify-center pt-4">
          <v-btn text @click="dialogoEliminacion = false">Cancelar</v-btn>
          <v-btn color="error" class="rounded-lg px-8 ml-2" @click="eliminarRegistro" :loading="eliminando">Confirmar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="snackbar.mostrar" :color="snackbar.color" :timeout="snackbar.timeout" rounded="pill" class="mb-4">
      <div class="d-flex align-center font-weight-bold">
        <v-icon dark class="mr-2">{{ snackbar.color === 'success' ? 'mdi-check-circle' : 'mdi-alert' }}</v-icon>
        {{ snackbar.mensaje }}
      </div>
    </v-snackbar>

    <v-overlay :value="cargando || guardando || eliminando" z-index="200" opacity="0.6">
      <v-progress-circular indeterminate size="70" width="7" color="primary"></v-progress-circular>
    </v-overlay>
  </v-container>
</template>

<script>
import moment from "moment";
import { mapActions, mapState } from "vuex";
import { mixin } from "../../mixin";

export default {
  name: "SaldoFijo",
  data: () => ({
    registros: [],
    filters: { titular: "", cuenta: "" },
    cargando: false,
    columnas: [
      { text: 'Titular', value: 'titular', sortable: true },
      { text: 'Cuenta', value: 'cuenta', sortable: true, width: '150px' },
      { text: 'Saldo Fijo', value: 'saldoFijo', sortable: true, align: 'right', width: '140px' },
      { text: 'Acciones', value: 'acciones', sortable: false, align: 'right', width: '130px' }
    ],
    dialogoDetalle: false,
    dialogoEliminacion: false,
    formularioValido: false,
    esEdicion: false,
    guardando: false,
    eliminando: false,
    registroEditando: { id: 0, titular: '', cuenta: '', saldoFijo: 0 },
    registroSeleccionado: {},
    registroAEliminar: {},
    reglas: {
      titular: [v => !!v || 'Requerido', v => (v && v.length >= 3) || 'Mínimo 3 caract.'],
      cuenta: [v => !!v || 'Requerido', v => (v && v.length >= 5) || 'Mínimo 5 caract.'],
      saldoFijo: [v => !!v || 'Requerido', v => (v && v >= 0) || 'Inválido']
    },
    snackbar: { mostrar: false, mensaje: '', color: 'success', timeout: 3000 }
  }),

  mixins: [mixin],

  filters: {
    currency(value) {
      if (!value && value !== 0) return "-";
      return new Intl.NumberFormat("es-MX", { style: "currency", currency: "MXN" }).format(value);
    }
  },

  mounted () {
    this.cargarRegistros();
    this.initFilters();
  },

  methods: {
    ...mapActions("inversion", ["getSaldoFijo", "postSaldoFijo", "putSaldoFijo", "deleteSaldoFijo"]),

    initFilters() {
      const f = {};
      this.columnas.forEach(c => { if(c.value !== 'acciones') f[c.value] = ""; });
      this.filters = f;
    },

    async cargarRegistros () {
      this.cargando = true;
      try {
        const response = await this.getSaldoFijo();
        this.registros = response || [];
      } catch (error) {
        this.mostrarMensaje('Error al obtener los datos', 'error');
      } finally {
        this.cargando = false;
      }
    },

    resetFormulario() {
      this.esEdicion = false;
      this.registroEditando = { id: 0, titular: '', cuenta: '', saldoFijo: 0 };
      this.$refs.formulario?.resetValidation();
    },

    editarRegistro (registro) {
      this.esEdicion = true;
      this.registroEditando = { ...registro };
      window.scrollTo({ top: 0, behavior: 'smooth' });
    },

    verDetalle (registro) {
      this.registroSeleccionado = { ...registro };
      this.dialogoDetalle = true;
    },

    confirmarEliminacion (registro) {
      this.registroAEliminar = { ...registro };
      this.dialogoEliminacion = true;
    },

    async guardarRegistro () {
      if (!this.$refs.formulario.validate()) return;
      this.guardando = true;
      try {
        const payload = {
          Id: this.registroEditando.id,
          Titular: this.registroEditando.titular,
          Cuenta: this.registroEditando.cuenta,
          SaldoFijo: parseFloat(this.registroEditando.saldoFijo)
        };

        if (this.esEdicion) {
          await this.putSaldoFijo(payload);
          this.mostrarMensaje('Actualizado con éxito');
        } else {
          await this.postSaldoFijo(payload);
          this.mostrarMensaje('Creado con éxito');
        }
        await this.cargarRegistros();
        this.resetFormulario();
      } catch (error) {
        this.mostrarMensaje('Error al guardar', 'error');
      } finally {
        this.guardando = false;
      }
    },

    async eliminarRegistro () {
      this.eliminando = true;
      try {
        await this.deleteSaldoFijo(this.registroAEliminar.id);
        this.mostrarMensaje('Eliminado correctamente');
        await this.cargarRegistros();
        this.dialogoEliminacion = false;
      } catch (error) {
        this.mostrarMensaje('Error al eliminar', 'error');
      } finally {
        this.eliminando = false;
      }
    },

    mostrarMensaje (mensaje, color = 'success') {
      this.snackbar = { mostrar: true, mensaje, color, timeout: 3000 };
    }
  },

  computed: {
    ...mapState("login", ["userName"]),
    filteredItems() {
      if (!Array.isArray(this.registros)) return [];
      return this.registros.filter(item => {
        return Object.keys(this.filters).every(key => {
          if (!this.filters[key]) return true;
          const val = String(item[key] || '').toLowerCase();
          const filter = String(this.filters[key]).toLowerCase();
          return val.includes(filter);
        });
      });
    },
    kpiWithBalance() { return this.registros.filter(r => (parseFloat(r.saldoFijo)||0) > 0).length; },
    kpiWithoutBalance() { return this.registros.filter(r => (parseFloat(r.saldoFijo)||0) <= 0).length; }
  }
};
</script>

<style scoped>
.fixo-dashboard { 
  background: #f8fafc !important; 
  min-height: 100vh; 
}
.theme--dark .fixo-dashboard { 
  background: #0f172a !important; 
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
.border-left-warning { border-left: 4px solid #FB8C00 !important; }

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
}
.theme--dark .premium-table >>> thead th { background: #252525 !important; }

.compact-filter-input >>> .v-input__slot { padding: 0 4px !important; border-radius: 4px !important; min-height: 20px !important; }
.sticky-panel { position: sticky; top: 88px; }
.bg-faint { background: rgba(0,0,0,0.01); }
.line-height-tight { line-height: 1.1; }
</style>
