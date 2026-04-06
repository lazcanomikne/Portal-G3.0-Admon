<template>
  <v-app id="login-portal">
    <v-main class="px-0 py-0">
      <v-container fluid class="pa-0 fill-height login-container">
        <v-row no-gutters class="fill-height">
          <!-- Left Side: Login Form with Glassmorphism -->
          <v-col cols="12" md="5" lg="4" class="d-flex align-center justify-center form-section">
            <div class="glass-panel px-8 py-12 rounded-xl animate-fade-in shadow-2xl">
              <div class="brand-header mb-8 text-center">
                <v-img 
                  :src="require('@/assets/govi_logo_black.png')" 
                  max-width="240" 
                  contain 
                  class="mx-auto mb-6 logo-glow"
                ></v-img>
                
                <h1 class="text-h4 font-weight-black white--text mb-2 tracking-tight">Bienvenido</h1>
                <p class="subtitle-1 white--text font-weight-medium">
                  Acceso a portal <span class="black--text font-weight-black">Operativo</span> y <span class="black--text font-weight-black">administrativo</span>
                </p>
              </div>

              <v-form v-model="valid" ref="form" @submit.prevent="submit" class="animate-slide-up">
                <v-text-field
                  v-model="username"
                  label="Usuario"
                  placeholder="Escriba su usuario"
                  :rules="emailRules"
                  required
                  solo
                  flat
                  rounded
                  background-color="white"
                  prepend-inner-icon="person"
                  class="mb-4 custom-field-premium"
                  light
                ></v-text-field>

                <v-text-field
                  v-model="password"
                  label="Contraseña"
                  placeholder="••••••••"
                  :type="showPassword ? 'text' : 'password'"
                  :append-icon="showPassword ? 'visibility' : 'visibility_off'"
                  @click:append="showPassword = !showPassword"
                  :rules="passwordRules"
                  required
                  solo
                  flat
                  rounded
                  background-color="white"
                  prepend-inner-icon="lock"
                  class="mb-6 custom-field-premium"
                  light
                  @keydown.enter="submit"
                ></v-text-field>

                <v-btn
                  block
                  x-large
                  elevation="8"
                  class="login-btn py-7"
                  @click="submit"
                  :loading="overlay"
                  :disabled="!valid"
                >
                  <span class="font-weight-black black--text">INICIAR SESIÓN</span>
                  <v-icon right color="black">arrow_forward</v-icon>
                </v-btn>
              </v-form>

              <div class="footer-note mt-10 text-center opacity-60">
                <p class="text-caption white--text">
                  &copy; {{ new Date().getFullYear() }} Portal GOVI. Todos los derechos reservados.
                </p>
              </div>
            </div>
          </v-col>

          <!-- Right Side: Visual & Particles -->
          <v-col cols="12" md="7" lg="8" class="visual-section hidden-sm-and-down">
            <!-- Particles Canvas -->
            <canvas id="spider-web-canvas" ref="canvas"></canvas>
          </v-col>
        </v-row>

        <v-overlay :value="overlay" z-index="999">
          <v-progress-circular indeterminate size="64" color="white"></v-progress-circular>
        </v-overlay>
      </v-container>
    </v-main>
  </v-app>
</template>

<script>
import { mapActions } from "vuex";

export default {
  name: "Login",
  data() {
    return {
      valid: false,
      overlay: false,
      password: "",
      passwordRules: [(v) => !!v || "Contraseña es requerida"],
      username: "",
      emailRules: [(v) => !!v || "Usuario es requerida"],
      showPassword: false,
      // Particles Variables
      canvas: null,
      ctx: null,
      particles: [],
      particleCount: 150,
      mouse: { x: null, y: null },
      animationId: null
    };
  },
  mounted() {
    this.initParticles();
    window.addEventListener("resize", this.handleResize);
    // Ensure clean state on login page to prevent unauthorized redirects on refresh
    localStorage.removeItem("jwt");
    localStorage.removeItem("govi_session");
  },
  beforeDestroy() {
    cancelAnimationFrame(this.animationId);
    window.removeEventListener("resize", this.handleResize);
  },
  methods: {
    ...mapActions("login", ["login"]),
    
    submit() {
      if (this.$refs.form.validate()) {
        this.overlay = true;
        this.login({ UserName: this.username, Password: this.password })
          .then((res) => {
            if (res) {
              // Restore original session key for router compatibility
              localStorage.setItem(
                "jwt",
                escape(JSON.stringify({ user: this.username, jwt: this.password }))
              );
              
              const sessionData = { 
                user: this.username, 
                loginTime: new Date().getTime() 
              };
              localStorage.setItem("govi_session", JSON.stringify(sessionData));
              
              this.$store.commit("login/SET_USERNAME", this.username);
              this.$store.commit("login/SET_USERPASS", this.password);
              this.$router.push("/");
            }
          })
          .catch((err) => {
            console.error(err);
            alert("Error de autenticación. Verifique sus credenciales.");
          })
          .finally(() => {
            this.overlay = false;
          });
      }
    },

    // --- SPIDERWEB PARTICLES LOGIC ---
    initParticles() {
      this.canvas = this.$refs.canvas;
      if (!this.canvas) return;
      this.ctx = this.canvas.getContext("2d");
      this.handleResize();

      this.particles = [];
      for (let i = 0; i < this.particleCount; i++) {
        this.particles.push({
          x: Math.random() * this.canvas.width,
          y: Math.random() * this.canvas.height,
          vx: (Math.random() - 0.5) * 0.4,
          vy: (Math.random() - 0.5) * 0.4,
          radius: Math.random() * 1.5 + 0.5
        });
      }

      window.addEventListener("mousemove", (e) => {
        const rect = this.canvas.getBoundingClientRect();
        this.mouse.x = e.clientX - rect.left;
        this.mouse.y = e.clientY - rect.top;
      });

      this.animate();
    },

    handleResize() {
      if (!this.canvas) return;
      const parent = this.canvas.parentElement;
      this.canvas.width = parent.clientWidth;
      this.canvas.height = parent.clientHeight;
    },

    animate() {
      if (!this.ctx) return;
      this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
      
      this.particles.forEach((p, i) => {
        p.x += p.vx;
        p.y += p.vy;

        if (p.x < 0 || p.x > this.canvas.width) p.vx *= -1;
        if (p.y < 0 || p.y > this.canvas.height) p.vy *= -1;

        // Draw particle
        this.ctx.beginPath();
        this.ctx.arc(p.x, p.y, p.radius, 0, Math.PI * 2);
        this.ctx.fillStyle = "rgba(255, 255, 255, 0.5)";
        this.ctx.fill();

        // Connect particles
        for (let j = i + 1; j < this.particles.length; j++) {
          const p2 = this.particles[j];
          const dist = Math.hypot(p.x - p2.x, p.y - p2.y);
          if (dist < 120) {
            this.ctx.beginPath();
            this.ctx.strokeStyle = `rgba(255, 255, 255, ${0.15 * (1 - dist / 120)})`;
            this.ctx.lineWidth = 0.5;
            this.ctx.moveTo(p.x, p.y);
            this.ctx.lineTo(p2.x, p2.y);
            this.ctx.stroke();
          }
        }

        // Mouse interaction
        const mDist = Math.hypot(p.x - this.mouse.x, p.y - this.mouse.y);
        if (mDist < 180) {
           this.ctx.beginPath();
           this.ctx.strokeStyle = `rgba(248, 161, 2, ${0.4 * (1 - mDist / 180)})`;
           this.ctx.lineWidth = 1;
           this.ctx.moveTo(p.x, p.y);
           this.ctx.lineTo(this.mouse.x, this.mouse.y);
           this.ctx.stroke();
        }
      });

      this.animationId = requestAnimationFrame(this.animate);
    }
  }
};
</script>

<style scoped>
.login-container {
  overflow: hidden;
  background-image: url('~@/assets/login_visual.jpg');
  background-size: cover;
  background-position: center;
}

.form-section {
  position: relative;
  z-index: 10;
  background: rgba(0, 0, 0, 0.3); /* Dark overlay for depth */
  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);
  border-right: 1px solid rgba(255, 255, 255, 0.1);
}

.glass-panel {
  background: linear-gradient(135deg, rgba(248, 161, 2, 0.7) 0%, rgba(255, 193, 7, 0.6) 100%);
  backdrop-filter: blur(25px) saturate(180%);
  -webkit-backdrop-filter: blur(25px) saturate(180%);
  border: 1px solid rgba(255, 255, 255, 0.3);
  box-shadow: 0 8px 32px 0 rgba(0, 0, 0, 0.3) !important;
  width: 100%;
  max-width: 420px;
}

.logo-glow {
  filter: drop-shadow(0 0 15px rgba(255,255,255,0.4));
}

/* Redefined Input Styles for maximum visibility */
.custom-field-premium >>> .v-input__control {
  min-height: 56px !important;
}

.custom-field-premium >>> .v-input__slot {
  background: white !important;
  box-shadow: 0 4px 20px rgba(0,0,0,0.1) !important;
  border: 2px solid transparent !important;
  transition: all 0.3s;
}

.custom-field-premium >>> input {
  color: #333 !important;
  font-weight: 600 !important;
  font-size: 1.1rem !important;
}

.custom-field-premium.v-input--is-focused >>> .v-input__slot {
  border-color: #000 !important;
}

.custom-field-premium >>> .v-label {
  color: #666 !important;
  font-weight: bold !important;
}

.custom-field-premium >>> .v-icon {
  color: #f8a102 !important;
}

.login-btn {
  background: white !important;
  color: black !important;
  border-radius: 14px !important;
  letter-spacing: 1px;
  text-transform: uppercase !important;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.login-btn:hover {
  transform: translateY(-3px);
  box-shadow: 0 12px 25px rgba(0, 0, 0, 0.2) !important;
  background: #fdfdfd !important;
}

.visual-section {
  position: relative;
  overflow: hidden;
}

#spider-web-canvas {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 2;
  pointer-events: none;
}

/* Animations */
.animate-fade-in {
  animation: fadeIn 0.8s ease-out;
}

.animate-slide-up {
  animation: slideUp 0.8s ease-out 0.2s both;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes slideUp {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}

.opacity-80 { opacity: 0.8; }
.opacity-60 { opacity: 0.6; }
</style>
