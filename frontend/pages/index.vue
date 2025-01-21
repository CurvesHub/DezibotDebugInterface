<template>
    <div class="h-screen flex items-center justify-center">
        <div class="w-1/2 h-4/6">
            <UCard >
                <template #header>
                    <div class="text-xl font-bold">{{ $t("session_picker_heading") }}</div>
                </template>
                
                <div class="flex flex-row">
                    <UModal v-model="isSessionChooserOpen">
                        <UCommandPalette 
                            v-model="selected" 
                            :groups="groups" 
                            class="flex-grow" 
                            :autoselect="false"
                            :placeholder="$t('search_placeholder')"
                            :close-button="{ icon: 'i-heroicons-x-mark-20-solid', color: 'gray', variant: 'link', padded: false }"
                            @close="isSessionChooserOpen = false"
                            @update:model-value="onSelected"
                        />
                    </UModal>

                    <UModal v-model="isNewSessionNamerOpen">
                        <UCard>
                            {{ $t("session_picker_create_session_name_prompt") }}
                            <UInput 
                                color="white" 
                                variant="outline" 
                                :placeholder="$t('session_picker_create_session_name_placeholder')"
                                @change="createNewSession(); isNewSessionNamerOpen = false"
                                v-model="newSessionName"
                                class="mt-2"
                            />
                        </UCard>
                    </UModal>

                    <UButton 
                        :label="selected?.label ?? $t('session_picker_text')"
                        class="flex-grow" 
                        color="white" 
                        @click="isSessionChooserOpen=true"
                        :loading="isLoading"
                    />

                    <div class="text-xl font-bold">
                        <UButton
                            icon="i-heroicons-trash"
                            size="sm"
                            color="red"
                            variant="solid"
                            label=""
                            class="ml-2"
                            :trailing="false"
                            @click="deleteSession"
                            :disabled="selected == undefined && !isLoading"
                        />
                    </div>
                </div>
                
                <template #footer>
                    <div class="flex flex-row-reverse">
                        <UButton 
                        :label="$t('session_picker_connect_button')" 
                        class="h-8" 
                        trailing-icon="i-heroicons-arrow-right-circle" 
                        :disabled="selected == undefined && !isLoading"
                        @click="connectToSession"
                    />
                    </div>
                </template>
            </UCard>
        </div>
    </div>
</template>

<script setup lang="ts">
    import { type Session } from '~/types/Session'
    const { t } = useI18n()
    type SessionEntry = {id: number, label: string}
    const { data } = await useFetch<Session[]>("/api/session/list")
    const sessions = ref<SessionEntry[]>(data.value?.map((e: any) => {return {id: e.id, label: e.name}}) ?? [])
    const groups = computed(() => [
        {key: 'commands', commands: [{id:-1, label: t("session_picker_create_session"), icon: "i-heroicons-plus"}]},
        {key:'sessions', commands: sessions.value}
    ])
    const selected = ref<SessionEntry>()
    const isSessionChooserOpen = ref(false)
    const isNewSessionNamerOpen = ref(false)
    const isLoading = ref(false)
    const newSessionName = ref("")

    function onSelected(selected: any) {
        isSessionChooserOpen.value = false
        if (selected.id == -1) {
            isSessionChooserOpen.value = false
            isNewSessionNamerOpen.value = true
        }
    }

    async function createNewSession() {
        isLoading.value = true
        const {data} = await useFetch<Session>("/api/session/new", {method:"post", body: {name: newSessionName.value}} as object)
        const id = data.value?.id
        const label = data.value?.name
        if (id && label) {
            const newSession: SessionEntry = {id: id, label: label}
            sessions.value.push(newSession)
            selected.value = newSession 
        } else {
            selected.value = undefined
        }
        newSessionName.value = ""
        isLoading.value = false
    }

    async function deleteSession() {
        isLoading.value = true
        const {data} = await useFetch(`/api/session/${selected?.value?.id}`, {method: "delete"} as object)
        const index = sessions.value.findIndex(session => session.id == selected?.value?.id)
        if (index > -1) {
            sessions.value.splice(index, 1)
            selected.value = sessions.value.at(index-1)
        }
        isLoading.value = false
    }

    function connectToSession() {
        navigateTo(`/session/${selected.value?.id}`)
    }
</script>