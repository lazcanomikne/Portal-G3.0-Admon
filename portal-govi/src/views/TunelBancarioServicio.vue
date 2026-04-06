<template>
  <v-container>
    <v-toolbar
      dense
      id="p"
      class="glass-toolbar mb-4"
    >
      <v-toolbar-title class="font-weight-bold">Validación de Folios para Pago de servicios</v-toolbar-title>
      <v-spacer> </v-spacer>
      <v-btn
        depressed
        class="glass-btn"
        @click="enviarTunel"
        :disabled="!selectedTxt.length"
      >
        Enviar a túnel bancario
      </v-btn>
    </v-toolbar>
    <div>
      <v-row dense>
        <v-col
          class="d-flex"
          cols="6"
        >
          <v-file-input
            label="Seleccionar archivos"
            accept="text/txt"
            outlined
            multiple
            dense
            class="glass-input"
            @change="onFileChange"
            v-model="selectedFile"
          ></v-file-input>
        </v-col>
        <v-col
          class="d-flex"
          cols="3"
          v-if="getRegistros.length > 0"
        >
          Archivos cargados:
          {{getRegistros.length}}
        </v-col>
        <v-col
          class="d-flex"
          cols="3"
          v-if="selectedTxt.length > 0"
        >
          Archivos seleccionados:
          {{selectedTxt.length}}
        </v-col>
      </v-row>
      <v-row dense>
        <v-col class="d-flex">
          <v-expansion-panels
            accordion
            focusable
          >
            <v-expansion-panel
              v-for="(item,i) in getRegistros"
              :key="i"
            >
              <v-expansion-panel-header>
                <v-row no-gutters>
                  <v-col cols="1">
                    <v-checkbox
                      hide-details
                      class="shrink mr-2 mt-0"
                      dense
                      style="margin-top: 0px"
                      v-model="selectedTxt"
                      :value="item.name"
                      @click.stop=""
                    ></v-checkbox>
                  </v-col>
                  <v-col cols="9">
                    {{item.name}} - {{item.nombre}}
                  </v-col>
                  <v-col cols="2">
                    {{item.totalImporte | currency}}
                  </v-col>
                </v-row>
              </v-expansion-panel-header>
              <v-expansion-panel-content>

                <v-data-table
                  dense
                  :headers="responseColumns"
                  :items="item.filas"
                  hide-default-footer
                  class="glass-table elevation-1"
                  item-key="referencia"
                  loading="true"
                >
                </v-data-table>
              </v-expansion-panel-content>
            </v-expansion-panel>
          </v-expansion-panels>
        </v-col>
      </v-row>
    </div>
    <v-overlay
      style="text-align: center"
      :value="overlay"
    >
      <p></p>
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
      <p>{{Respuesta}}</p>
      <v-btn
        depressed
        color="primary"
        @click="showResult = false"
      >
        Aceptar
      </v-btn>
    </v-overlay>
  </v-container>
</template>

<script>
import { mapActions } from 'vuex'
export default {
  name: 'TunelBancarioServicio',
  data: () => ({
    selectedFile: undefined,
    motivo: '',
    overlay: false,
    selectedTxt: [],
    showAlert: false,
    showResult: false,
    Respuesta: '',
    responseColumns: [
      { text: 'No. de emisora', value: 'referencia' },
      { text: 'Cuenta cargo', value: 'cuentaOrigen' },
      { text: 'Referencia', value: 'referencia1' },
      { text: 'Importe', value: 'importe', align: 'end' },
      { text: 'Fecha', value: 'fechaAplicacion', align: 'end' },
    ],
  }),
  methods: {
    ...mapActions("tunel", ['postTunelServicio', 'postUploadServicio']),
    enviarTunel () {
      this.overlay = true
      this.postTunelServicio(this.selectedTxt)
        .then(res => {
          if (res) {
            this.overlay = false
            this.showResult = true
            this.Respuesta = res.data
            this.selectedFile = undefined
          }
        })
        .catch(err => {
          this.overlay = false
          alert(err)
          console.error(err)
        })
        .finally(() => {
          this.overlay = false
        })
    },
    onFileChange (event) {
      if (!this.selectedFile) return
      this.selectedTxt = []
      this.overlay = true
      var formData = new FormData()
      this.selectedFile.forEach(element => {
        formData.append('files', element)
      });
      this.postUploadServicio(formData)
        .then(res => {
          if (res) {
            this.overlay = false
            this.selectedFile = undefined
          }
        })
        .catch(err => {
          this.overlay = false
          alert(err)
          console.error(err)
        })
        .finally(() => {
          this.selectedTxt = []
          this.overlay = false
        })
    }
  },
  computed: {
    getRegistros () {
      return this.$store.state.tunel.resultado
    }
  }
}
</script>

<style>
</style>