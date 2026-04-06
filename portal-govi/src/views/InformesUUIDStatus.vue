<template>
  <v-container>
    <v-toolbar dense class="glass-toolbar mb-4">
      <v-toolbar-title class="font-weight-bold">UUID's Estatus de Cancelación</v-toolbar-title>
      <v-spacer> </v-spacer>
      <export-excel class="v-btn v-btn--depressed theme--dark v-size--default primary" :data="sd_aldia"
        worksheet="Datos" :name="`reportedia${date}.xls`" v-if="sd_aldia.length">
        <v-icon>
          cloud_download
        </v-icon>
      </export-excel>
      <v-tooltip bottom>
        <template v-slot:activator="{ on, attrs }">
          <v-btn color="primary" dark v-bind="attrs" v-on="on" @click="Actualizar">
            <v-icon>update</v-icon>
          </v-btn>
        </template>
        <span>Actualizar pendientes</span>
      </v-tooltip>
    </v-toolbar>
    <v-row dense>
      <v-col justify="center" cols="12">
        <v-data-table dense v-model="selected" :headers="headers" :items="sd_aldia" :search="search"
          :items-per-page="15" item-key="cuenta" class="glass-table elevation-1" ref="table">
          <template v-slot:top>
            <v-row dense>
              <v-col cols="auto">
                <v-dialog ref="dialog" v-model="modal" :return-value.sync="date" persistent width="290px">
                  <template v-slot:activator="{ on, attrs }">
                    <v-text-field v-model="date" label="Fecha de consulta" prepend-icon="event" class="glass-input" readonly v-bind="attrs"
                      v-on="on"></v-text-field>
                  </template>
                  <v-date-picker v-model="date" scrollable>
                    <v-spacer></v-spacer>
                    <v-btn text color="primary" @click="modal = false">
                      Cancel
                    </v-btn>
                    <v-btn text color="primary" @click="cargarDatos">
                      OK
                    </v-btn>
                  </v-date-picker>
                </v-dialog>
              </v-col>
              <v-col cols="12" sm="12" md="10">
                <v-text-field v-model="search" label="Filtrar resultados" class="glass-input mx-4"></v-text-field>
              </v-col>
            </v-row>
          </template>
          <template v-slot:[`item.total_UUID`]="{ item }">
            <span> {{ item.total_UUID | currency }} </span>
          </template>
          <template v-slot:[`item.estatus`]="{ item }">
            <v-chip color="green" dark> {{ item.estatus }} </v-chip>
          </template>
        </v-data-table>
      </v-col>
    </v-row>
    <v-overlay style="text-align: center" :value="overlay">
      <p>Procesando...</p>
      <v-progress-circular indeterminate size="64"></v-progress-circular>
    </v-overlay>
  </v-container>
</template>

<script>
import { mapActions } from "vuex";

export default {
  data() {
    return {
      search: "",
      modal: false,
      date: new Date().toISOString().substr(0, 10),
      selected: [],
      overlay: false,
      headers: [
        { text: "Emisor", value: "rfC_Emisor" },
        { text: "Receptor", value: "rfC_Receptor" },
        { text: "UUID", value: "uuid" },
        { text: "Total", value: "total_UUID", align: "right" },
        { text: "Estado", value: "estatus" },
        { text: "Motivo", value: "motivo" },
      ],
    };
  },
  mounted() {
    this.cargarDatos();
  },
  methods: {
    ...mapActions("informes", { getInfo: "getUUIDStatus" }),
    ...mapActions("cancelacion", ["putCancelacion"]),
    cargarDatos() {
      this.$refs.dialog.save(this.date)
      this.overlay = true;
      this.getInfo(this.date)
        .then(() => (this.overlay = false))
        .catch(() => (this.overlay = false))
        .finally(() => (this.overlay = false));
    },
    Actualizar() {
      this.overlay = true;
      this.putCancelacion()
      .then((res) => {
          if (res) {
            this.overlay = false;
            alert("Proceso terminado...")
          }
        })
        .catch((err) => {
          this.overlay = false;
          console.error(err);
        })
        .finally(() => {
          this.overlay = false;
        });
    }
  },
  computed: {
    sd_aldia() {
      return this.$store.state.informes.uuidlist;
    },
  },
};
</script>

<style></style>
