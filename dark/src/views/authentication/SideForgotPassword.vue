<script setup lang="ts">
import Logo from '@/layouts/full/logo/LogoAuth.vue';
/* Reset form */
import ResetPassword from '@/components/auth/ResetForm.vue';
</script>

<template>
    <div class="pa-3">
        <v-row class="h-100vh auth">
            <v-col cols="12" lg="5" xl="4" class="d-flex align-center justify-center bg-surface">
                <div class="mt-xl-0 mt-5 mw-450">
                    <div class="auth-header pa-6">
                        <div class="position-relative">
                            <Logo />
                        </div>
                    </div>
                    <div class="pa-sm-7 pa-4 pb-0">
                        <h2 class="textPrimary text-h4 font-weight-semibold">Forgot your password?</h2>
                        <p class="card-subtitle py-4 ">
                            Please enter the email address associated with your account and We will email you a link to reset your password.
                        </p>
                        <ResetPassword />
                        <v-btn size="large" color="lightprimary" to="/auth/login" block class="mt-5 text-primary" flat>Back to Login</v-btn>
                    </div>
                </div>
            </v-col>
            <v-col cols="12" lg="7" xl="8" class="d-none d-lg-flex align-center justify-center authentication position-relative">
                <div class="">
                    <img src="@/assets/images/backgrounds/login-bg.png" class="position-relative" alt="login-background" />
                </div>
            </v-col>
        </v-row>
    </div>
</template>
