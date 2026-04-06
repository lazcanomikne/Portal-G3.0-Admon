<template>
  <v-container>
    <v-toolbar dense class="glass-toolbar mb-4">
      <v-toolbar-title class="font-weight-bold">Informe de bancos</v-toolbar-title>
      <v-spacer> </v-spacer>
      <export-excel
        class="v-btn v-btn--depressed theme--dark v-size--default primary"
        :data="sd_aldia"
        worksheet="Datos"
        :name="`reportedia${date}.xls`"
        v-if="sd_aldia.length"
      >
        <v-icon>
          cloud_download
        </v-icon>
      </export-excel>
    </v-toolbar>
    <v-row dense>
      <v-col
        cols="12"
        sm="6"
        md="4"
      >
        <v-dialog
          ref="dialog"
          v-model="modal"
          :return-value.sync="date"
          persistent
          width="290px"
        >
          <template v-slot:activator="{ on, attrs }">
            <v-text-field
              v-model="date"
              label="Fecha de consulta"
              prepend-icon="event"
              class="glass-input"
              readonly
              v-bind="attrs"
              v-on="on"
            ></v-text-field>
          </template>
          <v-date-picker
            v-model="date"
            scrollable
          >
            <v-spacer></v-spacer>
            <v-btn
              text
              color="primary"
              @click="modal = false"
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
    </v-row>
    <v-row dense>
      <v-col
        justify="center"
        cols="12"
      >
        <v-data-table
          dense
          v-model="selected"
          :headers="headers"
          :items="sd_aldia"
          :search="search"
          :items-per-page="15"
          item-key="cuenta"
          class="glass-table elevation-1"
          ref="table"
        >
          <template v-slot:top>
            <v-text-field
              v-model="search"
              label="Filtrar resultados"
              class="glass-input mx-4"
            ></v-text-field>
          </template>
          <template v-slot:[`item.saldoDiario`]="{ item }">
            <span> {{item.saldoDiario | currency}} </span>
          </template>
        </v-data-table>
      </v-col>
    </v-row>
    <v-overlay
      style="text-align: center"
      :value="overlay"
    >
      <p>Calculando saldo diario</p>
      <v-progress-circular
        indeterminate
        size="64"
      ></v-progress-circular>
    </v-overlay>
  </v-container>
</template>

<script>
import { mapActions } from 'vuex'

export default {
  data () {
    return {
      search: '',
      modal: false,
      date: new Date().toISOString().substr(0, 10),
      selected: [],
      overlay: false,
      headers: [
        { text: 'Empresa', value: 'empresa' },
        { text: 'Cuenta', value: 'cuenta' },
        { text: 'Nombre Cuenta', value: 'nombreCuenta' },
        { text: 'Saldo Diario', value: 'saldoDiario' },
        { text: 'Fecha', value: 'fecha' },//Si el usuario va a consultar para que mostrarla para que al exportar a excel y guarde no pierda el control, siempre pensemos que el usuario mexicano es un tanto tonto
      ],
    }
  },
  methods: {
    ...mapActions("informes", { getInfo: 'getDatos' }),
    cargarDatos () {
      this.$refs.dialog.save(this.date)
      this.overlay = true
      this.getInfo(this.date.replaceAll('-', ''))
        .then(() => this.overlay = false)
        .catch(() => this.overlay = false)
        .finally(() => this.overlay = false)
    },
  },
  computed: {
    sd_aldia () {
      return this.$store.state.informes.sdaldia
    }
  }
}
</script>

<style>
</style>