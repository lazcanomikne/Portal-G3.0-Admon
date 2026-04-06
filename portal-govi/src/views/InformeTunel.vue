<template>
  <v-container>
    <v-toolbar dense class="glass-toolbar mb-4">
      <v-toolbar-title class="font-weight-bold">Informe tunel bancario</v-toolbar-title>
    </v-toolbar>
    <v-row dense>
      <v-col
        cols="6"
        sm="6"
        md="4"
      >
        <v-dialog
          ref="dialog1"
          v-model="modal1"
          :return-value.sync="dateFin"
          persistent
          width="290px"
        >
          <template v-slot:activator="{ on, attrs }">
            <v-text-field
              v-model="dateFin"
              label="Fecha final"
              prepend-icon="event"
              class="glass-input"
              readonly
              v-bind="attrs"
              v-on="on"
            ></v-text-field>
          </template>
          <v-date-picker
            v-model="dateFin"
            scrollable
          >
            <v-spacer></v-spacer>
            <v-btn
              text
              color="primary"
              @click="modal1 = false"
            >
              Cancel
            </v-btn>
            <v-btn
              text
              color="primary"
              @click="cargarDatos"
            >
              OK
            </v-btn>
          </v-date-picker>
        </v-dialog>
      </v-col>
      <v-col
        cols="6"
        md="6"
      >
        <v-btn
          depressed
          class="glass-btn"
          style="margin-right: 10px"
          @click="showResult = true"
          :disabled="!lenDif > 0"
        >Ver errores</v-btn>
      </v-col>
    </v-row>
    <v-row
      dense
      justify="space-around"
    >
      <v-col
        v-for="rounded in getStatistics"
        :key="rounded.title"
        cols="12"
        md="3"
      >
        <v-sheet
          rounded
          class="mx-auto"
        >
          <v-list>
            <v-list-item two-line>
              <v-list-item-avatar>
                {{rounded.total}}
              </v-list-item-avatar>
              <v-list-item-content>
                <v-list-item-title>{{rounded.title.split(" ")[0]}}</v-list-item-title>
                <v-list-item-subtitle>{{rounded.title}} </v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
          </v-list>
        </v-sheet>
      </v-col>
    </v-row>
    <v-row dense>
      <v-col
        class="text-center"
        cols="12"
        md="12"
      >
        <v-expansion-panels accordion>
          <v-expansion-panel
            v-for="(item,i) in getRegistros"
            :key="i"
          >
            <v-expansion-panel-header>
              <v-row no-gutters>
                <v-col cols="2">
                  {{item.folio}}
                </v-col>
                <v-col cols="3">
                  {{item.fecha}}
                </v-col>
                <v-col cols="5">
                  {{item.confirmacion}}
                </v-col>
                <v-col cols="2">
                  {{item.sucursal}}
                </v-col>
              </v-row>
            </v-expansion-panel-header>
            <v-expansion-panel-content>
              <v-data-table
                dense
                :headers="responseColumns"
                :items="item.detalle"
                hide-default-footer
                disable-pagination
                disable-sort
                class="glass-table elevation-1"
                item-key="id"
                loading="true"
              >
                <template v-slot:[`item.importe`]="{ item }">
                  <span> {{item.importe | currency}} </span>
                </template>
              </v-data-table>
            </v-expansion-panel-content>
          </v-expansion-panel>
        </v-expansion-panels>
      </v-col>
    </v-row>
    <v-overlay
      style="text-align: center"
      :value="overlay"
    >
      <p>Generando informe</p>
      <v-progress-circular
        indeterminate
        size="64"
      ></v-progress-circular>
    </v-overlay>
    <v-overlay
      style="text-align: center"
      :z-index="10"
      :value="showResult"
    >
      <v-toolbar
        dense
        color="primary"
      >
        <v-spacer></v-spacer>
        <v-toolbar-title>Folios no procesados</v-toolbar-title>
        <v-spacer></v-spacer>
      </v-toolbar>
      <v-row>
        <v-col cols="12">
          <v-data-table
            dense
            :headers="responseColumns2"
            :items="getDiferencias"
            :height="tableHeight"
            hide-default-footer
            disable-pagination
            disable-sort
            class="glass-table elevation-1"
            item-key="docEntry"
            loading="true"
          >
            <template v-slot:[`item.importe`]="{ item }">
              <span> {{item.importe | currency}} </span>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
      <br>
      <v-btn
        depressed
        color="primary"
        @click="showResult = false"
      >
        Cerrar
      </v-btn>
    </v-overlay>
  </v-container>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'

export default {
  data () {
    return {
      search: '',
      modal: false,
      modal1: false,
      dateIni: new Date().toISOString().substr(0, 10),
      dateFin: new Date().toISOString().substr(0, 10),
      selected: [],
      overlay: false,
      showResult: false,
      responseColumns: [
        { text: '#', value: 'id' },
        { text: 'Operacion', value: 'operacion' },
        { text: 'Descripcion', value: 'descripcion' },
        { text: 'Cuenta Origen', value: 'cuentaOrigen' },
        { text: 'Cuenta Destino', value: 'cuentaDestino' },
        { text: 'Referencia', value: 'referencia' },
        { text: 'Importe', value: 'importe', align: 'end' },
        { text: 'Fecha Ejecucion', value: 'fechaEjecucion', align: 'end' },
        { text: 'Titular', value: 'titulardelaCuenta' },
        { text: 'Confirmacion', value: 'confirmacion' },
      ],
      responseColumns2: [
        { text: '#', value: 'id' },
        { text: 'Folio', value: 'folio' },
        { text: 'Sucursal', value: 'sucursal' },
        { text: 'Descripcion', value: 'descripcion' },
        { text: 'Cuenta Origen', value: 'cuentaOrigen' },
        { text: 'Cuenta Destino', value: 'cuentaDestino' },
        { text: 'Referencia', value: 'referencia' },
        { text: 'Importe', value: 'importe', align: 'end' },
        { text: 'Fecha Ejecucion', value: 'fechaEjecucion', align: 'end' },
        { text: 'Titular', value: 'titulardelaCuenta' },
      ],
    }
  },
  methods: {
    ...mapActions("tunel", ['getInforme']),
    setDateIni () {
      this.$refs.dialog.save(this.dateIni)
    },
    cargarDatos () {
      this.$refs.dialog1.save(this.dateFin)
      const [year, month, day] = this.dateFin.split('-')
      this.overlay = true
      this.getInforme({ FechaFin: this.dateFin, FechaIni: `${day}${month}${year}` })
        .then(() => this.overlay = false)
        .catch(() => this.overlay = false)
        .finally(() => this.overlay = false)
    },
  },
  computed: {
    ...mapGetters("tunel", ['doneRows', 'lenDif']),
    getRegistros () {
      return this.doneRows.rows
    },
    getStatistics () {
      return this.doneRows.statistics
    },
    tableHeight () {
      return window.innerHeight - 300;
    },
    getDiferencias () {
      return this.doneRows.diferencias
    }
  }
}
</script>

<style>
</style>