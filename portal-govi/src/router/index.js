import Vue from "vue";
import VueRouter from "vue-router";

Vue.use(VueRouter);

const routes = [
  {
    name: "login",
    path: "/login",
    component: () => import("../layouts/Login.vue"),
    meta: { desc: "Iniciar sesión", guest: true },
  },
  {
    path: "/",
    component: () => import("../layouts/MainLayout.vue"),
    meta: {
      requiresAuth: true,
    },
    children: [
      {
        path: "/",
        name: "Home",
        component: () => import("../views/Home.vue"),
        meta: { desc: "Inicio " },
      },
      {
        path: "/bancos/dispersion",
        name: "Dispersion",
        component: () => import("../views/Dispersion.vue"),
        meta: { desc: "Dispersion " },
      },
      {
        path: "/bancos/dispersionservicios",
        name: "DispersionServicio",
        component: () => import("../views/DispersionServicio.vue"),
        meta: { desc: "Dispersion de servicios" },
      },
      {
        path: "/bancos/dispersion_1",
        name: "Dispersion 1 a 1",
        component: () => import("../views/Dispersion_1.vue"),
        meta: { desc: "Dispersion 1 a 1" },
      },
      {
        path: "/bancos/dispersion_state",
        name: "ActualizarDispersion",
        component: () => import("../views/Dispersion_state.vue"),
        meta: { desc: "Actualizar Dispersión" },
      },
      {
        path: "/informes/informebancos",
        name: "Informes",
        component: () => import("../views/InformesBancos.vue"),
        meta: { desc: "Informes" },
      },
      {
        path: "/informes/generados",
        name: "FoliosGenerados",
        component: () => import("../views/FoliosGenerados.vue"),
        meta: { desc: "Folios Generados" },
      },
      {
        path: "/bancos/tunel",
        name: "TunelBancario",
        component: () => import("../views/TunelBancario.vue"),
        meta: { desc: "Tunel Bancario" },
      },
      {
        path: "/bancos/tunelservicio",
        name: "TunelBancarioServicio",
        component: () => import("../views/TunelBancarioServicio.vue"),
        meta: { desc: "Tunel Bancario Servicio" },
      },
      {
        path: "/informes/informetunel",
        name: "InformeTunel",
        component: () => import("../views/InformeTunel.vue"),
        meta: { desc: "Informe Tunel Bancario" },
      },
      {
        path: "/informes/reportetransfers",
        name: "InformeTransfer",
        component: () => import("../views/InformeTransfer.vue"),
        meta: { desc: "Informe Transferencias" },
      },
      {
        path: "/informe/uuidstatus",
        name: "InformesUUIDStatus",
        component: () => import("../views/InformesUUIDStatus.vue"),
        meta: {
          desc: "UUID´s Estatus de Cancelacion",
        },
      },
      {
        path: "/inventario/ajustesalida",
        name: "AjusteSalidaInventario",
        component: () => import("../views/AjusteSalida.vue"),
        meta: { desc: "Ajuste de salida de inventario" },
      },
      {
        path: "/inventario/ajusteentrada",
        name: "AjusteEntradaInventario",
        component: () => import("../views/AjusteEntrada.vue"),
        meta: { desc: "Ajuste de entrada de inventario" },
      },
      {
        path: "/creditocobranza/notadebito",
        name: "NotaDebito",
        component: () => import("../views/Credito/NotaDebito.vue"),
        meta: { desc: "Nota debito" },
      },
      {
        path: "/creditocobranza/pagos",
        name: "Pagos",
        component: () => import("../views/Credito/Pagos.vue"),
        meta: { desc: "Pagos FA/NC" },
      },
      {
        path: "/creditocobranza/autorizacion-de-preaplicaciones",
        name: "AutorizacionPreaplicaciones",
        component: () =>
          import("../views/Credito/AutorizacionPreaplicaciones.vue"),
        meta: { desc: "Autorizacion de Prealplicaciones" },
      },
      {
        path: "/creditocobranza/informe-de-operaciones",
        name: "InformeOperaciones",
        component: () =>
          import("../views/Credito/InformeOperacionesAplicadas.vue"),
        meta: { desc: "Informe de Operaciones" },
      },
      {
        path: "/bancos/pagofilieales",
        name: "PagoFiliales",
        component: () => import("../views/PagoFiliales.vue"),
        meta: { desc: "Pago de filiales" },
      },
      {
        path: "/bancos/pagosefectuados",
        name: "PagosAfectados",
        component: () => import("../views/PagosEfectuados.vue"),
        meta: { desc: "Pagos Efectuados" },
      },
      {
        path: "/task/cancelacionuuid",
        name: "CancelacionUUID",
        component: () => import("../views/CancelacionUUID.vue"),
        meta: { desc: "Cancelacion Masiva" },
      },
      {
        path: "/informe/cuadroinversion",
        name: "CuadroInversion",
        component: () => import("../views/InformeCuadroInversion.vue"),
        meta: { desc: "Cuadro de Inversión" },
      },
      {
        path: "/informe/analisiscuadroinversion",
        name: "AnalisisCuadroInversion",
        component: () => import("../views/Credito/AnalisisCuadroInversion.vue"),
        meta: { desc: "Análisis Cuadro de Inversión" },
      },
      {
        path: "/inversion/saldodisponible",
        name: "SaldoDisponible",
        component: () => import("../views/Inversion/SaldoDisponible.vue"),
        meta: { desc: "Saldo disponible" },
      },
      {
        path: "/inversion/saldofijo",
        name: "SaldoFijo",
        component: () => import("../views/Inversion/SaldoFijo.vue"),
        meta: { desc: "Saldo fijo" },
      },
      {
        path: "/inversion/apartados",
        name: "Apartados",
        component: () => import("../views/Inversion/Apartados.vue"),
        meta: { desc: "Apartados" },
      },
      {
        path: "/admin/usuarios",
        name: "Usuarios",
        component: () => import("@/views/Admin/Usuarios.vue"),
        meta: { desc: "Usuarios y Permisos" },
      },
      {
        path: "/admin/config-cuentas",
        name: "ConfigCuentas",
        component: () => import("../views/Inversion/ConfigCuentas.vue"),
        meta: { desc: "Configuración de Cuentas" },
      },
      {
        path: "/admin/usuarios-permisos",
        name: "UsuariosPermisos",
        component: () => import("../views/Administracion/UsuariosPermisos.vue"),
        meta: { desc: "Gestión de Usuarios y Permisos" },
      },
      // Always leave this as last one,
      // but you can also remove it
      {
        path: "*",
        component: () => import("../views/Informes.vue"),
      },
    ],
  },
  {
    path: "/about",
    name: "About",
    component: () => import("../views/About.vue"),
    desc: "About",
  },
];

const router = new VueRouter({
  scrollBehavior: () => ({ x: 0, y: 0 }),
  mode: "hash",
  base: process.env.BASE_URL,
  routes,
});
router.beforeEach((to, from, next) => {
  document.title = to.meta.desc;
  if (to.path == "/login") {
    localStorage.removeItem("jwt");
    localStorage.removeItem("b1session");
    localStorage.removeItem("routeid");
    localStorage.removeItem("govi_session");
    next();
    return false;
  } else {
    if (to.matched.some((record) => record.meta.requiresAuth)) {
      if (localStorage.getItem("jwt") == null) {
        localStorage.setItem("nextUrl", to.fullPath);
        next({ name: "login" });
      } else {
        next();
      }
    } else if (to.matched.some((record) => record.meta.guest)) {
      if (localStorage.getItem("jwt") != null) {
        next({ path: "/" });
      } else {
        next();
      }
    } else {
      next();
    }
  }
});

export default router;
