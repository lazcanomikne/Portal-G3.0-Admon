<script setup lang="ts">
import Logo from '@/layouts/full/logo/LogoAuth.vue';
/*form component*/
import TwoStepForm from '@/components/auth/TwoStepForm.vue';
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
                    <div class="pa-sm-6 pa-4 w-100">
                        <h3 class="text-h4 textPrimary font-weight-semibold">Two Step Verification</h3>
                        <p class="card-subtitle mt-2 text-13">
                            We sent a verification code to your mobile. Enter the code from the mobile in the field below.
                        </p>
                        <h6 class="card-subtitle mt-3 font-weight-medium">******1234</h6>
                        <!---Form---->
                        <TwoStepForm />
                        <!------->
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
