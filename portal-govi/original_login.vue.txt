<template>
  <v-app id="inspire">
    <v-content>
      <v-container
        fluid
        fill-height
        class="loginOverlay"
      >
        <v-layout
          flex
          align-center
          justify-center
        >
          <v-flex
            xs12
            sm3
            elevation-6
          >
            <v-toolbar class="pt-5 blue darken-4">
              <v-toolbar-title class="white--text">
                <h4>Portal GOVI</h4>
              </v-toolbar-title>
            </v-toolbar>
            <v-card>
              <v-card-text class="pt-4">
                <div>
                  <v-form
                    v-model="valid"
                    ref="form"
                  >
                    <v-text-field
                      label="Usuario"
                      v-model="username"
                      :rules="emailRules"
                      required
                    ></v-text-field>
                    <v-text-field
                      label="Contraseña"
                      v-model="password"
                      type="password"
                      :rules="passwordRules"
                      counter
                      required
                    ></v-text-field>
                    <v-layout justify-space-between>
                      <v-btn
                        @click="submit"
                        :class=" { 'blue darken-4 white--text' : valid, disabled: !valid }"
                      >Entrar</v-btn>
                    </v-layout>
                  </v-form>
                </div>
              </v-card-text>
            </v-card>
          </v-flex>
        </v-layout>
      </v-container>
    </v-content>
  </v-app>
</template>

<script>
export default {
  name: "login",
  data () {
    return {
      valid: false,
      e1: false,
      password: '',
      passwordRules: [
        (v) => !!v || 'Contraseña is required',
      ],
      username: '',
      emailRules: [
        (v) => !!v || 'Usuario is required'
      ],
    }
  },
  methods: {
    submit () {
      if (this.$refs.form.validate()) {
        this.$store.dispatch("login", { UserName: this.username, Password: this.password })
          .then(res => {
            if (res) {
              localStorage.setItem('jwt', escape(JSON.stringify({ email: this.email, jwt: this.password })))
              localStorage.setItem('user', this.username)
              this.$router.push('/')
            }
            this.loadTable = false
          })
      }
    },
    clear () {
      this.$refs.form.reset()
    }
  },
}
</script>

<style>
</style>