document.addEventListener('DOMContentLoaded', () => {
    const toggleBtn = document.getElementById('inline-edit-toggle');
    if (!toggleBtn) return;

    let isEditMode = false;
    let floatingSaveBtn = null;

    toggleBtn.addEventListener('click', () => {
        isEditMode = !isEditMode;
        document.body.classList.toggle('edit-mode', isEditMode);
        
        if (isEditMode) {
            enableEditMode();
            toggleBtn.classList.replace('bg-v-yellow', 'bg-v-green');
            toggleBtn.classList.replace('text-v-black', 'text-white');
        } else {
            disableEditMode();
            toggleBtn.classList.replace('bg-v-green', 'bg-v-yellow');
            toggleBtn.classList.replace('text-white', 'text-v-black');
        }
    });

    function enableEditMode() {
        // Find all editable elements
        const editables = document.querySelectorAll('[data-editable]');
        editables.forEach(el => {
            if (el.tagName === 'IMG') {
                setupImageEditor(el);
            } else {
                el.contentEditable = "true";
                // Prevent links from navigating while editing
                if (el.tagName === 'A' || el.closest('a')) {
                    el.addEventListener('click', preventNav);
                }
            }
        });

        // Add Floating Save Button
        if (!floatingSaveBtn) {
            floatingSaveBtn = document.createElement('button');
            floatingSaveBtn.textContent = 'Save Changes';
            floatingSaveBtn.className = 'delivery-float';
            floatingSaveBtn.style.right = 'auto';
            floatingSaveBtn.style.left = '24px';
            floatingSaveBtn.innerHTML = '<i class="bi bi-floppy"></i> <span>SAVE</span>';
            floatingSaveBtn.onclick = saveChanges;
            document.body.appendChild(floatingSaveBtn);
        }
        floatingSaveBtn.style.display = 'block';
    }

    function disableEditMode() {
        const editables = document.querySelectorAll('[data-editable]');
        editables.forEach(el => {
            if (el.tagName === 'IMG') {
                teardownImageEditor(el);
            } else {
                el.contentEditable = "false";
                el.removeEventListener('click', preventNav);
            }
        });

        if (floatingSaveBtn) {
            floatingSaveBtn.style.display = 'none';
        }
    }

    function preventNav(e) {
        e.preventDefault();
    }

    function setupImageEditor(imgEl) {
        if (imgEl.dataset.hasEditor === 'true') return;
        imgEl.dataset.hasEditor = 'true';

        const parent = imgEl.parentElement;
        const currentPos = window.getComputedStyle(parent).position;
        if (currentPos === 'static') {
            parent.style.position = 'relative';
        }
        
        if (parent.tagName === 'A') {
            parent.addEventListener('click', preventNavWrapper);
        }

        const fileInput = document.createElement('input');
        fileInput.type = 'file';
        fileInput.accept = 'image/*';
        fileInput.className = 'hidden';
        const fileId = 'file-' + Math.random().toString(36).substr(2, 9);
        fileInput.id = fileId;
        parent.appendChild(fileInput);

        const positionKey = imgEl.getAttribute('data-position-key');

        const overlay = document.createElement('div');
        overlay.className = 'img-edit-overlay absolute inset-0 bg-black/50 flex-col gap-3 items-center justify-center opacity-0 hover:opacity-100 transition-opacity z-50 m-0 hidden';
        // We use hidden/flex toggle because inline-editor sets it to block by default? No, the original was a single label.
        // We will manage its display carefully.
        overlay.style.display = 'flex';
        
        const changeLabel = document.createElement('label');
        changeLabel.htmlFor = fileId;
        changeLabel.className = 'flex items-center gap-2 text-white font-bold cursor-pointer hover:text-v-yellow transition-colors border border-white px-4 py-2 rounded-md hover:bg-white/20 bg-black/50 backdrop-blur-sm';
        changeLabel.innerHTML = '<i class="bi bi-upload"></i> Change Image';
        changeLabel.onclick = (e) => { e.stopPropagation(); };
        overlay.appendChild(changeLabel);

        if (positionKey) {
            const adjustBtn = document.createElement('button');
            adjustBtn.className = 'flex items-center gap-2 text-white font-bold cursor-pointer hover:text-v-yellow transition-colors border border-white px-4 py-2 rounded-md hover:bg-white/20 bg-black/50 backdrop-blur-sm';
            adjustBtn.innerHTML = '<i class="bi bi-hand-index-thumb"></i> Adjust Image';
            adjustBtn.onclick = (e) => {
                e.stopPropagation();
                e.preventDefault();
                startPanMode(imgEl, parent, overlay, positionKey);
            };
            overlay.appendChild(adjustBtn);
        }

        parent.appendChild(overlay);

        overlay.onclick = (e) => {
            e.stopPropagation();
        };

        let cropperInstance = null;

        fileInput.onchange = (e) => {
            const file = e.target.files[0];
            if (!file) return;

            const modalOverlay = document.getElementById('crop-modal-overlay');
            const imageTarget = document.getElementById('crop-image-target');
            const cancelBtn = document.getElementById('crop-cancel-btn');
            const saveBtn = document.getElementById('crop-save-btn');
            
            if (!modalOverlay || !imageTarget || typeof Cropper === 'undefined') {
                uploadImageDirectly(file, imgEl);
                return;
            }

            const reader = new FileReader();
            reader.onload = (re) => {
                imageTarget.src = re.target.result;
                modalOverlay.classList.remove('hidden');
                
                if (cropperInstance) {
                    cropperInstance.destroy();
                }
                
                cropperInstance = new Cropper(imageTarget, {
                    viewMode: 1,
                    autoCropArea: 1,
                    background: false
                });

                cancelBtn.onclick = () => {
                    modalOverlay.classList.add('hidden');
                    if (cropperInstance) {
                        cropperInstance.destroy();
                        cropperInstance = null;
                    }
                    fileInput.value = '';
                };

                saveBtn.onclick = () => {
                    if (!cropperInstance) return;
                    
                    saveBtn.innerHTML = '<i class="bi bi-hourglass-split"></i> Uploading...';
                    saveBtn.disabled = true;

                    const mimeType = file.type === 'image/png' ? 'image/png' : 'image/jpeg';
                    
                    cropperInstance.getCroppedCanvas({
                        imageSmoothingEnabled: true,
                        imageSmoothingQuality: 'high'
                    }).toBlob(async (blob) => {
                        const csrfToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
                        const formData = new FormData();
                        
                        formData.append('image', blob, file.name);
                        formData.append('__RequestVerificationToken', csrfToken);
                        
                        try {
                            const response = await fetch('/api/cms/upload', {
                                method: 'POST',
                                headers: { 'RequestVerificationToken': csrfToken },
                                body: formData
                            });
                            
                            if (response.ok) {
                                const result = await response.json();
                                if (result.imageUrl) {
                                    imgEl.src = result.imageUrl;
                                    imgEl.setAttribute('data-new-url', result.imageUrl);
                                    // Auto-save so the user doesn't lose the image on refresh
                                    if (floatingSaveBtn) {
                                        floatingSaveBtn.click();
                                    }
                                }
                            } else {
                                alert('Failed to upload cropped image.');
                            }
                        } catch (err) {
                            console.error(err);
                            alert('Upload error.');
                        } finally {
                            modalOverlay.classList.add('hidden');
                            if (cropperInstance) {
                                cropperInstance.destroy();
                                cropperInstance = null;
                            }
                            fileInput.value = '';
                            saveBtn.innerHTML = '<i class="bi bi-crop"></i> Crop & Upload';
                            saveBtn.disabled = false;
                        }
                    }, mimeType, 1.0);
                };
            };
            reader.readAsDataURL(file);
        };

        async function uploadImageDirectly(file, imgEl) {
            const reader = new FileReader();
            reader.onload = (re) => {
                imgEl.src = re.target.result;
            };
            reader.readAsDataURL(file);

            const formData = new FormData();
            formData.append('image', file);
            
            const csrfToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
            formData.append('__RequestVerificationToken', csrfToken);

            try {
                const response = await fetch('/api/cms/upload', {
                    method: 'POST',
                    headers: { 'RequestVerificationToken': csrfToken },
                    body: formData
                });
                if (response.ok) {
                    const result = await response.json();
                    if (result.imageUrl) {
                        imgEl.setAttribute('data-new-url', result.imageUrl);
                    }
                } else {
                    alert('Failed to upload image.');
                }
            } catch (err) {
                console.error(err);
                alert('Upload error.');
            }
        }
    }

    function preventNavWrapper(e) {
        if (document.body.classList.contains('edit-mode')) {
            if (e.target.tagName === 'INPUT') return;
            e.preventDefault();
        }
    }

    function teardownImageEditor(imgEl) {
        imgEl.dataset.hasEditor = 'false';
        const parent = imgEl.parentElement;
        const overlay = parent.querySelector('.img-edit-overlay');
        const fileInput = parent.querySelector('input[type="file"]');
        if (overlay) overlay.remove();
        if (fileInput) fileInput.remove();
        if (parent.tagName === 'A') {
            parent.removeEventListener('click', preventNavWrapper);
        }
    }

    function startPanMode(imgEl, parent, overlay, positionKey) {
        overlay.style.display = 'none';
        
        const doneBtn = document.createElement('button');
        doneBtn.textContent = 'Save Position';
        doneBtn.className = 'absolute bottom-4 right-4 bg-v-green text-white font-bold px-4 py-2 rounded-md shadow-lg z-50 cursor-pointer hover:bg-green-600 transition-colors';
        parent.appendChild(doneBtn);
        
        let isDragging = false;
        let startX, startY;
        
        let currentPos = window.getComputedStyle(imgEl).objectPosition || '50% 50%';
        let parts = currentPos.split(' ');
        let posX = parseFloat(parts[0]);
        let posY = parseFloat(parts[1]);
        if (isNaN(posX)) posX = 50;
        if (isNaN(posY)) posY = 50;
        
        imgEl.style.cursor = 'move';
        
        const onMouseDown = (e) => {
            isDragging = true;
            startX = e.clientX;
            startY = e.clientY;
            e.preventDefault();
        };
        
        const onMouseMove = (e) => {
            if (!isDragging) return;
            const deltaX = e.clientX - startX;
            const deltaY = e.clientY - startY;
            startX = e.clientX;
            startY = e.clientY;
            
            const rect = parent.getBoundingClientRect();
            // Sensitivity multiplier, 1.5 feels more natural than 1.0
            const moveX = (deltaX / rect.width) * 150;
            const moveY = (deltaY / rect.height) * 150;
            
            posX -= moveX;
            posY -= moveY;
            
            posX = Math.max(0, Math.min(100, posX));
            posY = Math.max(0, Math.min(100, posY));
            
            imgEl.style.objectPosition = `${posX}% ${posY}%`;
        };
        
        const onMouseUp = () => {
            isDragging = false;
        };
        
        parent.addEventListener('mousedown', onMouseDown);
        window.addEventListener('mousemove', onMouseMove);
        window.addEventListener('mouseup', onMouseUp);
        
        doneBtn.onclick = async (e) => {
            e.stopPropagation();
            e.preventDefault();
            
            parent.removeEventListener('mousedown', onMouseDown);
            window.removeEventListener('mousemove', onMouseMove);
            window.removeEventListener('mouseup', onMouseUp);
            imgEl.style.cursor = '';
            doneBtn.remove();
            
            overlay.style.display = 'flex';
            
            const csrfToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
            const newPos = `${Math.round(posX)}% ${Math.round(posY)}%`;
            
            try {
                const response = await fetch('/api/cms/update', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': csrfToken
                    },
                    body: JSON.stringify([{
                        entityType: 'SiteContent',
                        entityId: positionKey,
                        field: 'Value',
                        value: newPos
                    }])
                });
                
                if (!response.ok) {
                    alert('Failed to save image position.');
                }
            } catch (err) {
                console.error(err);
                alert('Save error.');
            }
        };
    }

    async function saveChanges() {
        const updates = [];
        const editables = document.querySelectorAll('[data-editable]');
        
        editables.forEach(el => {
            const entityType = el.getAttribute('data-entity-type');
            const entityId = el.getAttribute('data-entity-id');
            const field = el.getAttribute('data-editable');
            
            let value;
            if (el.tagName === 'IMG') {
                value = el.getAttribute('data-new-url');
                if (!value) return; // No new image
            } else {
                value = el.innerText.trim();
            }

            updates.push({
                EntityType: entityType,
                EntityId: entityId,
                Field: field,
                Value: value
            });
        });

        if (updates.length === 0) {
            alert('No changes to save.');
            return;
        }

        floatingSaveBtn.innerHTML = '<i class="bi bi-hourglass-split"></i> <span>SAVING...</span>';

        const csrfToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';

        try {
            const response = await fetch('/api/cms/update', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': csrfToken
                },
                body: JSON.stringify(updates)
            });

            if (response.ok) {
                floatingSaveBtn.innerHTML = '<i class="bi bi-check-lg"></i> <span>SAVED!</span>';
                setTimeout(() => {
                    floatingSaveBtn.innerHTML = '<i class="bi bi-floppy"></i> <span>SAVE</span>';
                }, 1500);
            } else {
                alert('Failed to save changes.');
                floatingSaveBtn.innerHTML = '<i class="bi bi-floppy"></i> <span>SAVE</span>';
            }
        } catch (err) {
            console.error(err);
            alert('Error saving changes.');
            floatingSaveBtn.innerHTML = '<i class="bi bi-floppy"></i> <span>SAVE</span>';
        }
    }
});
