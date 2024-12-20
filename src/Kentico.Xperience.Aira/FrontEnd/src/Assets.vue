<template>
    <div class="c-app_inner" v-bind:class="{'is-loaded':isLoaded}">
        <div id="loading" class="c-loading">
            <img :src="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/spinner.svg`" class="c-loading_spinner" alt="loading spinner" />
        </div>

        <div class="c-app_header">
            <NavBarComponent :airaPathBase="pathsModel.pathsBase" :navBarModel="navBarModel" :baseUrl="baseUrl" />
        </div>

        <template v-if="phase == 'uploading'">
            <div id="loading" class="c-loading">
                <img :src="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/spinner.svg`" class="c-loading_spinner" alt="loading spinner" />
            </div>

            <div class="c-done-page-layout">
                <div>
                    <h2>Thank you<span class="text-primary">.</span></h2>
                </div>
                <div>
                    <svg viewBox="0 0 78.400003 58.227815"
                         xmlns="http://www.w3.org/2000/svg" class="c-checkmark">
                        <polyline class="path"
                                  fill="none"
                                  stroke-width="8"
                                  stroke-linecap="round"
                                  stroke-miterlimit="10"
                                  stroke-dasharray="100"
                                  stroke-dashoffset="-100"
                                  points="74.4,4 25.7,52.6 4,31.5"
                                  id="polyline1" />
                    </svg>
                </div>
                <div>
                    <p>
                        Your images have been successfully uploaded.
                    </p>
                </div>
                <div>
                    <a href="/aira/assets" class="btn btn-primary text-uppercase">
                        <svg class="c-icon fs-6 me-2">
                            <use :xlink:href="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/icons.svg#plus-circle`"></use>
                        </svg>
                        upload more
                    </a>
                </div>
            </div>
        </template>

    <div class="c-app_body">
        <div class="container">
            <form>
                <input ref="fileInput" hidden type="file" accept=".jpg,.jpeg,.png,.bmp" class="d-none" multiple>
            </form>
            <template v-if="phase == 'empty'">
                <div class="c-empty-page-layout">
                    <div>
                        <p>It looks empty, please upload your images here.</p>
                    </div>
                    <div>
                        <img :src="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/image-placeholder.svg`" alt="image placeholder" class="img-fluid">
                    </div>
                    <div>
                        <button class="btn btn-secondary text-uppercase" @click="pickImage();">
                            <svg class="c-icon fs-6">
                                <use :xlink:href="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/icons.svg#plus-circle`"></use>
                            </svg>
                            upload assets
                        </button>
                    </div>
                </div>
            </template>

            <template v-if="phase == 'selection'">
                <div class="c-message_wrapper u-secondary-accent" id="messages">
                    <div class="c-message">
                        <div class="c-message_bubble white w-100">
                            <div class="row g-2.5">
                                <div class="col-6" v-for="(file, index) in files" :key="index">
                                    <div class="c-message_img ratio ratio-1x1 position-relative">
                                        <img class="object-fit-cover" v-bind:src="createObjectURL(file)" alt="uploaded image">
                                        <button class="btn btn-remove-img" aria-label="Remove" @click="removeFile2(file, index);">
                                            <svg class="c-icon">
                                                <use xlink:href="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/icons.svg#cross`"></use>
                                            </svg>
                                        </button>
                                    </div>
                                </div>

                                <div class="c-add-asset-div">
                                    <button class="btn btn-secondary text-uppercase" @click="pickImage();">
                                        <svg class="c-icon fs-6">
                                            <use xlink:href="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/icons.svg#plus-circle`"></use>
                                        </svg>
                                        upload assets
                                    </button>
                                </div>
                            </div>

                            <div class="form-check mt-3">
                                <input class="form-check-input" id="meta-tagging-checkbox" name="meta-tagging-checkbox" type="checkbox" v-model="useMetaTagging" />
                            </div>

                            <div class="mt-3">
                                <p class="c-note">AIRA will automatically assign meta tags for all of your pictures</p>
                            </div>
                        </div>
                    </div>
                </div>
            </template>
            <div class="mt-5" v-if="phase == 'selection'">
                <button type="button" class="btn btn-primary btn-lg w-100" :disabled="!formIsValid" @click="fireUpload()">
                    UPLOAD FILES
                </button>
            </div>
        </div>
    </div>
    </div>
</template>

<script>
    import NavBarComponent from "./Navigation.vue";

    export default {
        components: {
            NavBarComponent
        },
        props: {
            pathsModel: null,
            baseUrl: null,
            navBarModel: null
        },
        data() {
            return {
                // NG STATE
                files: [],
                phase: 'empty', // 'uploading', 'selection', 'empty'
                formIsValid: true,
                currentStreamingIndex: -1,
                uploading: false,
                useMetaTagging: false,
                filesPromiseResolve: null,
                filesPromise: null,

                // old state
                uploadInstance: {},
                dbg: {},
                title: "",
                productDetailCta: "",
                languageCode: 'EN',
                navigation: [],
                isLoaded: true
            }
        },
        mounted() {
            document.onreadystatechange = () => {
                if (document.readyState === "complete") {
                    this.main();
                }
            }
        },
        methods: {
            main() {
                this.validateState();
                this.$refs.fileInput.onchange = this.fileInputChange;

                setTimeout(() => {
                    var modal = document.querySelector('#loading');
                    if (modal) {
                        modal.classList.add('is-hidden');
                    }
                    setTimeout(function () {
                        modal.parentNode.removeChild(modal);
                    }, 500);
                }, 1000);
            },
            async pickImage(event) {
                this.phase = 'selection';
                this.filesPromise = new Promise(resolve => this.filesPromiseResolve = resolve);
                this.$refs.fileInput.click();
                const files = await this.filesPromise;
                this.filesPromise = null;
                this.filesPromiseResolve = null;
                Array.from(files).forEach(file => {
                    this.files.push(file);
                });
                this.validateState();
            },
            createObjectURL(file) {
                return URL.createObjectURL(file);
            },
            fileInputChange(e) {
                this.filesPromiseResolve(e.target.files);
            },
            validateState() {
                this.formIsValid = this.files.length > 0;
            },
            removeFile2(file, index) {
                this.files.splice(index, 1);
                this.validateState();
            },
            fireUpload() {
                const uploadCreateBody = {
                    files: []
                };
                this.files.forEach((f, idx) => {
                    uploadCreateBody.files.push({ fileId: null, fileName: f.name, index: idx });
                });
                uploadCreateBody.useMetaTagging = this.useMetaTagging;

                fetch(`@baseUrl/aira/upload`, {
                    method: 'POST',
                    body: JSON.stringify(uploadCreateBody),
                    mode: "same-origin",
                    credentials: "same-origin",
                    headers: {
                        "Content-Type": "application/json",
                    },
                })
                    .then(async (r) => {
                        this.uploadInstance = await r.json();
                        this.phase = 'uploading';

                        setTimeout(() => {
                            var modal = document.querySelector('#loading');

                            if (modal) {
                                modal.classList.remove('is-hidden');
                            }
                            setTimeout(function () {
                                modal.parentNode.removeChild(modal);
                                setTimeout(() => {
                                    if (document.querySelector(".c-checkmark")) {
                                        document.querySelector(".c-checkmark").classList.add("do-animation");
                                    }
                                })
                            }, 500);
                        }, 1000);

                        const ui = this.uploadInstance;
                        const ids = [];
                        const data = new FormData();
                        this.files.forEach((f, idx) => {
                            data.append('files', f, f.name);
                            ids.push(ui.files[idx].fileId);
                        });
                        data.append("ids", ids);

                        const uploadId = ui.uploadId;
                        const route = `./fireUpload/${uploadId}`;
                        fetch(route, {
                            method: 'POST',
                            // mode: "same-origin",
                            // credentials: "same-origin",
                            body: data,
                        }).then(async (response) => {
                            const reader = response.body?.getReader();
                            while (!!reader) {
                                const { done, value } = await reader.read();
                                if (done) break;
                                let dec = new TextDecoder().decode(value);

                                const decCleared = dec.replace(/^[\[\],]*/gi, '').replace(/[\[\],]*$/gi, '');
                                let current = '';
                                let level = 0;
                                for (let letter of decCleared) {
                                    if (letter == '{') {
                                        level = level + 1;
                                    }

                                    if (level > 0) {
                                        current += letter;
                                    }

                                    if (letter == '}') {
                                        level = level - 1;
                                        if (level == 0) {
                                            const p = JSON.parse(current);
                                            const file = this.uploadInstance.files.find(f => f.fileId === p.fileId);
                                            if (file) {
                                                file.progress = p.progress;
                                                this.currentStreamingIndex = this.uploadInstance.files.indexOf(file);
                                                this.uploading = this.uploadInstance.files.reduce((acc, f) => acc || f.progress < 100, false);
                                            }
                                            current = '';
                                        }
                                    }
                                }
                            }
                        });
                    });
            },
            removeFile(file) {
                console.log(`removeFile`, file);
                const uploadId = this.uploadInstance?.uploadId;
                fetch(`./upload/${uploadId}/deleteFile/${file.fileId}`, {
                    method: 'DELETE',
                    body: JSON.stringify({ files: [{ fileId: null, fileName: null }] }),
                    mode: "same-origin",
                    credentials: "same-origin",
                    headers: {
                        "Content-Type": "application/json",
                        // 'Content-Type': 'application/x-www-form-urlencoded',
                    },
                })
                    .then(async (r) => {
                        this.uploadInstance.files.splice(this.uploadInstance.files.indexOf(file), 1);
                        this.dbg = JSON.stringify(this.uploadInstance, 2, 2);
                    })
            },
            addFile() {
                const uploadId = this.uploadInstance?.uploadId;
                const route = !uploadId ? `./upload/addfile` : `./upload/${uploadId}/addfile`;

                fetch(route, {
                    method: 'POST',
                    body: JSON.stringify({ files: [{ fileId: null, fileName: null }] }),
                    mode: "same-origin",
                    credentials: "same-origin",
                    headers: {
                        "Content-Type": "application/json",
                        // 'Content-Type': 'application/x-www-form-urlencoded',
                    },
                })
                    .then(async (r) => {
                        const uploadInstance = await r.json();
                        this.uploadInstance = uploadInstance;
                        this.dbg = JSON.stringify(uploadInstance, 2, 2);
                    });
            },
            startUpload() {
                const ui = this.uploadInstance;
                const data = new FormData();

                const ids = [];
                ui.files.forEach((f, idx) => {
                    if (f.reftoinput) {
                        const offset = f?.offset;
                        if (offset) {
                            data.append('files', f.reftoinput.slice(offset), f.reftoinput.name);
                        } else {
                            data.append('files', f.reftoinput, f.reftoinput.name);
                        }
                        ids.push(f.fileId);
                    }
                });

                data.append("ids", ids);

                const uploadId = this.uploadInstance?.uploadId;
                const route = `./upload/${uploadId}/fire`;
                fetch(route, {
                    method: 'POST',
                    // mode: "same-origin",
                    // credentials: "same-origin",
                    body: data,
                }).then(async (response) => {
                    const reader = response.body?.getReader();
                    while (!!reader) {
                        const { done, value } = await reader.read();
                        if (done) break;
                        let dec = new TextDecoder().decode(value);

                        const decCleared = dec.replace(/^[\[\],]*/gi, '').replace(/[\[\],]*$/gi, '');
                        console.log(decCleared);
                        let current = '';
                        let level = 0;
                        for (let letter of decCleared) {
                            if (letter == '{') {
                                level = level + 1;
                            }

                            if (level > 0) {
                                current += letter;
                            }

                            if (letter == '}') {
                                level = level - 1;
                                if (level == 0) {
                                    const p = JSON.parse(current);
                                    const file = this.uploadInstance.files.find(f => f.fileId === p.fileId);
                                    if (file) {
                                        file.progress = p.progress;
                                    }


                                    current = '';
                                }
                            }
                        }

                    }
                });
            }
        }
    }


</script>