<template>
    <nav class="navbar">
        <div class="container">
            <span v-if="displayLogo">
                <div class="navbar-brand">
                    <img alt="Kentico" id="aira-unified-app-logo" :src="`${this.baseUrl}${this.navBarModel?.logoImgRelativePath}`">
                </div>
            </span>
            <h1 class="c-app_title" id="aira-unified-title">
                <span v-if="displayLogo">
                    <svg id="aira-unified-title-img" class="c-icon" width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M5.61629 10.1095L7.48839 4.49316L9.36049 10.1095L14.9768 11.9816L9.36049 13.8537L7.48839 19.4699L5.61629 13.8537L0 11.9816L5.61629 10.1095Z" fill="currentColor" />
                        <path d="M13.8542 18.3463L14.9774 14.9766L16.1007 18.3463L19.4704 19.4696L16.1007 20.5929L14.9774 23.9626L13.8542 20.5929L10.4844 19.4696L13.8542 18.3463Z" fill="currentColor" />
                        <path d="M16.4735 4.49303L17.9712 0L19.4689 4.49303L23.9619 5.99071L19.4689 7.48839L17.9712 11.9814L16.4735 7.48839L11.9805 5.99071L16.4735 4.49303Z" fill="currentColor" />
                    </svg>

                    <span id="aira-unified-title-text">
                        {{ navBarModel?.titleText }}
                    </span>
                </span>
                <span v-if="!displayLogo">
                    <a :href="`${navBarModel.chatItem.url}`" class="c-app_title_link">
                        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M24.0051 12.75C24.0051 13.1653 23.6693 13.5019 23.2551 13.5019L2.57384 13.5019L5.78033 16.7164C6.07322 17.0101 6.07322 17.4861 5.78033 17.7798C5.48743 18.0734 5.01256 18.0734 4.71967 17.7798L0.350889 13.4C0.304301 13.3706 0.260239 13.3356 0.219668 13.2949C0.0699251 13.1448 -0.00325131 12.947 0.000118983 12.7502C-0.0033686 12.5533 0.0698135 12.3553 0.219668 12.2051C0.260239 12.1644 0.3043 12.1294 0.350886 12.1L4.71967 7.72022C5.01256 7.42659 5.48744 7.42659 5.78033 7.72022C6.07322 8.01385 6.07322 8.48992 5.78033 8.78355L2.57384 11.9981L23.2551 11.9981C23.6693 11.9981 24.0051 12.3347 24.0051 12.75Z" fill="currentColor" />
                        </svg>
                        <span id="aira-title-text">
                            <strong>{{ `${this.navBarModel?.titleText}` }}</strong>
                            [<i>Preview</i>]
                        </span>
                    </a>
                </span>
            </h1>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNavAltMarkup" aria-controls="navbarNavAltMarkup" aria-expanded="false" aria-label="Toggle navigation">
                <svg class="c-icon" id="burger-icon" :style="{ color: themeColor }">
                    <use :xlink:href="`${baseUrl}/_content/Kentico.Xperience.AiraUnified/img/icons.svg#hamburger-menu`"></use>
                </svg>
            </button>
        </div>

        <div class="collapse navbar-collapse" id="navbarNavAltMarkup">
            <div class="container">
                <div class="navbar-nav">
                    <div class="c-nav">
                        <div class="c-nav_content" id="aira-unified-menu-submenu-items">
                            <a class="d-flex align-items-center gap-2 btn" v-if="navBarModel?.chatItem" :href="`${navBarModel.chatItem.url}`">
                                <img :src="`${baseUrl}${navBarModel.chatItem.menuImage}`" class="c-icon text-primary" />
                                {{ navBarModel.chatItem.title }}
                            </a>
                            <a class="d-flex align-items-center gap-2 btn" v-if="navBarModel?.smartUploadItem" :href="`${navBarModel.smartUploadItem.url}`">
                                <img :src="`${baseUrl}${navBarModel.smartUploadItem.menuImage}`" class="c-icon text-primary" />
                                {{ navBarModel.smartUploadItem.title }}
                            </a>
                        </div>

                        <hr class="c-nav_hr">
                        <div class="c-nav_footer">
                            <p class="text-center fs-1">
                                {{ navBarModel?.menuMessage }}
                            </p>
                            <p id="appVersion" class="mt-3"></p>
                            <p>© 2024 Kentico. All rights reserved.</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </nav>
</template>

<script>
export default {
    props: {
        baseUrl: {
            type: String,
            required: true
        },
        airaUnifiedBaseUrl: {
            type: String,
            required: true
        },
        navigationUrl: {
            type: String,
            required: true
        },
        navigationPageIdentifier: {
            type: String,
            required: true
        },
        displayLogo: {
            type: Boolean,
            required: true
        } // Displays back to chat thread selector arrow if false
    },
    data() {
        return {
            themeColor: "#8107c1",
            navBarModel: null
        }
    },
    mounted() {
        this.retrieveNavBar();
    },
    methods: {
        async retrieveNavBar() {
            const response = await fetch(this.navigationUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    pageIdentifier: this.navigationPageIdentifier,
                }),
            });

            if (!response.ok) {
                console.error('Error fetching navigation data');
            }

            this.navBarModel = await response.json();
        }
    }
}
</script>
