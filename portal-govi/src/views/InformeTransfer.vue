<template>
  <v-container>
    <v-toolbar dense class="glass-toolbar mb-4">
      <v-toolbar-title class="font-weight-bold">Informe de transferencias</v-toolbar-title>
      <v-spacer> </v-spacer>
    </v-toolbar>
    <v-row dense>
      <v-col
        cols="12"
        sm="6"
        md="4"
      >
        <v-dialog
          ref="dialogStart"
          v-model="modalStart"
          :return-value.sync="dateStart"
          persistent
          width="290px"
        >
          <template v-slot:activator="{ on, attrs }">
            <v-text-field
              v-model="dateStart"
              label="Fecha inicial"
              prepend-icon="event"
              class="glass-input"
              readonly
              v-bind="attrs"
              v-on="on"
            ></v-text-field>
          </template>
          <v-date-picker
            v-model="dateStart"
            scrollable
          >
            <v-spacer></v-spacer>
            <v-btn
              text
              color="primary"
              @click="modalStart = false"
            >
              Cancel
            </v-btn>
            <v-btn
              text
              color="primary"
              @click="saveDateStart"
            >
              OK
            </v-btn>
          </v-date-picker>
        </v-dialog>
      </v-col>
      <v-col
        cols="12"
        sm="6"
        md="4"
      >
        <v-dialog
          ref="dialogEnd"
          v-model="modalEnd"
          :return-value.sync="dateEnd"
          persistent
          width="290px"
        >
          <template v-slot:activator="{ on, attrs }">
            <v-text-field
              v-model="dateEnd"
              label="Fecha final"
              prepend-icon="event"
              class="glass-input"
              readonly
              v-bind="attrs"
              v-on="on"
            ></v-text-field>
          </template>
          <v-date-picker
            v-model="dateEnd"
            scrollable
          >
            <v-spacer></v-spacer>
            <v-btn
              text
              color="primary"
              @click="modalEnd = false"
            >
              Cancel
            </v-btn>
            <v-btn
              text
              color="primary"
              @click="loadData"
            >
              OK
            </v-btn>
          </v-date-picker>
        </v-dialog>
      </v-col>
      <v-col
        cols="12"
        sm="6"
        md="4"
      >
        <v-btn
          depressed
          class="glass-btn"
          style="margin-right: 10px"
          @click="loadData"
        >Generar</v-btn>
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
          :items="infoTransfers"
          :search="search"
          :height="tableHeight"
          :fixed-header="true"
          hide-default-footer
          disable-pagination
          disable-sort
          item-key="cuenta"
          class="glass-table elevation-1"
          ref="table"
        >
          <template v-slot:top>
            <v-text-field
              @keydown.esc="search=''"
              v-model="search"
              label="Filtrar resultados"
              class="glass-input mx-4"
            ></v-text-field>
          </template>
          <template v-slot:[`item.importeTotal`]="{ item }">
            <span> {{item.importeTotal | currency}} </span>
          </template>
          <template v-slot:[`item.actions`]="{ item }">
            <v-icon
              small
              @click="loadDetails(item)"
            >
              pageview
            </v-icon>
          </template>
        </v-data-table>
      </v-col>
    </v-row>
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
        <v-toolbar-title>{{title}}</v-toolbar-title>
        <v-spacer></v-spacer>
      </v-toolbar>
      <v-row>
        <v-col cols="12">
          <v-data-table
            dense
            :headers="detailsHeaders"
            :items="detailsTransfers"
            :search="searchDetails"
            :height="tableHeight"
            :fixed-header="true"
            hide-default-footer
            disable-pagination
            disable-sort
            class="elevation-1"
            item-key="docEntry"
            loading="true"
          >
            <template v-slot:top>
              <v-text-field
                @keydown.esc="searchDetails=''"
                v-model="searchDetails"
                label="Filtrar resultados"
                class="mx-4"
              ></v-text-field>
            </template>
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
        @click="showResult = false; searchDetails = ''"
      >
        Cerrar
      </v-btn>
    </v-overlay>
    <v-overlay
      style="text-align: center"
      :value="overlay"
    >
      <p>{{loadingText}}</p>
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
      searchDetails: '',
      title: '',
      loadingText: '',
      modalStart: false,
      modalEnd: false,
      dateStart: new Date().toISOString().substr(0, 10),
      dateEnd: new Date().toISOString().substr(0, 10),
      selected: [],
      overlay: false,
      showResult: false,
      headers: [
        { text: 'Empresa', value: 'empresa' },
        { text: 'Sucursal', value: 'sucursal' },
        { text: 'NombreCuenta', value: 'nombreCuenta' },
        { text: '# Tranferencias', value: 'transferencias' },
        { text: 'Importe Total', value: 'importeTotal' },
        { text: 'Ver', value: 'actions' },
      ],
      detailsHeaders: [
        { text: 'NombreCuenta', value: 'nombreCuenta' },
        { text: "Proveedor", value: "numProv" },
        { text: "Beneficiario", value: "beneficiario" },
        { text: "Tranferencia", value: "transfer" },
        { text: "Cheque", value: "cHeque" },
        { text: "Emision", value: "fEmision" },
        { text: "Descripcion", value: "descripciondeDispersion" },
        { text: "Importe", value: "importe" },
        { text: "Domiciliado", value: "u_Domiciliado" },
        { text: "Fecha Act.", value: "fechaAct" }
      ],
    }
  },
  methods: {
    ...mapActions("informes", { getHeader: 'getInfoTransfers', getDetails: 'getDetailsTransfers' }),
    saveDateStart () {
      this.$refs.dialogStart.save(this.dateStart)
    },
    loadData () {
      this.$refs.dialogEnd.save(this.dateEnd)
      this.loadingText = 'Cargando registros de transferencias'
      this.overlay = true
      this.getHeader({ FechaIni: this.dateStart.replaceAll('-', ''), FechaFin: this.dateEnd.replaceAll('-', '') })
        .then(() => this.overlay = false)
        .catch(() => this.overlay = false)
        .finally(() => this.overlay = false)
    },
    loadDetails (item) {
      this.title = item.empresa
      this.loadingText = `Cargando detalles de ${this.title}`
      this.overlay = true
      this.getDetails({ fechas: { FechaIni: this.dateStart.replaceAll('-', ''), FechaFin: this.dateEnd.replaceAll('-', '') }, empresa: item.empresa })
        .then(() => {
          this.overlay = false
          this.showResult = true
        })
        .catch(() => this.overlay = false)
        .finally(() => this.overlay = false)
    }
  },
  computed: {
    tableHeight () {
      return window.innerHeight - 300;
    },
    infoTransfers () {
      return this.$store.state.informes.infoTransfers
    },
    detailsTransfers () {
      return this.$store.state.informes.detallTransfers
    }
  }
}
</script>

<style>
</style>