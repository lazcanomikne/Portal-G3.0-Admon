<script setup lang="ts">
import Logo from '@/layouts/full/logo/LogoAuth.vue';
/* Register form */
import RegisterForm from '@/components/auth/RegisterForm.vue';
</script>

<template>
    <div class="pa-3">
        <v-row class="h-100vh auth">
            <v-col cols="12" lg="5" xl="4" class="d-flex align-center justify-center bg-surface">
                <div class="mt-xl-0 mt-5 mw-100">
                    <div class="auth-header pa-6">
                        <div class="position-relative">
                            <Logo />
                        </div>
                    </div>
                    <h2 class="text-h3 textPrimary font-weight-semibold mb-2">Welcome to MaterialPro</h2>
                    <div class="card-subtitle mb-6">Your Admin Dashboard</div>
                    <RegisterForm />
                    <h6 class="text-h6 text-medium-emphasis d-flex align-center mt-6">
                        Already have an Account?
                        <v-btn variant="plain" to="/auth/login" class="text-primary text-body-1 opacity-1 pl-2">Sign In</v-btn>
                    </h6>
                </div>
            </v-col>
            <v-col cols="12" lg="7" xl="8" class="d-none d-lg-flex align-center justify-center authentication position-relative">

                <div class="">
                    <img src="@/assets/images/backgrounds/login-bg.png" class="position-relative " alt="login-background" />
                </div>
            </v-col>
        </v-row>
    </div>
</template>
