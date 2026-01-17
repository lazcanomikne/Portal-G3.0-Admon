<template>
    <VCard elevation="10" class="overflow-hidden">
        <v-card-text>
            <h3 class="card-title">Experiances</h3>
            <div class="mt-8">
                <div class="d-flex align-center ga-10"></div>
            </div>
            <div class="no-line">
                <v-table class="month-table">
                    <template v-slot:default>
                        <tbody>
                            <tr class="no-line">
                                <td style="width: 90px" class="">
                                    <img src="@/assets/images/browser/photoshop.jpg" alt="icon" />
                                </td>
                                <td style="width: 200px" class="">
                                    <h5 class="text-h5 text-no-wrap">Photoshop</h5>
                                    <p class="textSecondary text-subtitle-1 text-no-wrap">This is a sample text</p>
                                </td>
                                <td class="w-100">
                                    <div>
                                    <v-progress-linear
                                        model-value="65"
                                        color="primary"
                                        bg-color="grey200"
                                        rounded
                                        height="5"
                                        class="text-no-wrap"
                                    ></v-progress-linear>
                                </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 90px">
                                    <img src="@/assets/images/browser/sketch.jpg" alt="icon" />
                                </td>
                                <td>
                                    <h5 class="text-h5 text-no-wrap">Sketch</h5>
                                    <p class="textSecondary text-subtitle-1 text-no-wrap">This is a sample text</p>
                                </td>
                                <td class="w-100" >
                                    <div>
                                    <v-progress-linear
                                        model-value="45"
                                        color="secondary"
                                        bg-color="grey200"
                                        rounded
                                        height="5"
                                    ></v-progress-linear>
                                </div>
                                </td>
                            </tr>
                        </tbody>
                    </template>
                </v-table>
            </div>
        </v-card-text>
        <div class="pa-7 border-t">
            <v-row class="align-center">
                <v-col cols="6" md="4" class="text-center">
                    <h2 class="text-h2 font-weight-medium mb-1">5486</h2>
                    <h5 class="textSecondary font-weight-semibold">Projects</h5>
                </v-col>
                <v-col cols="6" md="4" class="text-center">
                    <h2 class="text-h2 font-weight-medium mb-1">987</h2>
                    <h5 class="textSecondary font-weight-semibold">Winning Entries</h5>
                </v-col>
                <v-col cols="12" md="4">
                    <v-btn color="primary" flat rounded="pill" class="w-100" >Hire Me</v-btn>
                </v-col>
            </v-row>
        </div>
    </VCard>
</template>
