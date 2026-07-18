// Corporate Website Main JavaScript

(function() {
    'use strict';

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        initBackToTop();
        initCookieConsent();
        initAOS();
        initSmoothScroll();
        initLazyLoading();
        initForms();
        initCounters();
        initTooltips();
        initDropdowns();
    });

    // Back to Top Button
    function initBackToTop() {
        const backToTop = document.getElementById('backToTop');
        if (!backToTop) return;

        window.addEventListener('scroll', function() {
            if (window.scrollY > 300) {
                backToTop.classList.add('show');
            } else {
                backToTop.classList.remove('show');
            }
        });

        backToTop.addEventListener('click', function(e) {
            e.preventDefault();
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }

    // Cookie Consent
    function initCookieConsent() {
        const cookieConsent = document.getElementById('cookie-consent');
        const acceptBtn = document.getElementById('cookie-accept');
        const declineBtn = document.getElementById('cookie-decline');

        if (!cookieConsent) return;

        // Check if consent already given
        if (localStorage.getItem('cookieConsent')) {
            return;
        }

        // Show after 1 second
        setTimeout(function() {
            cookieConsent.hidden = false;
        }, 1000);

        if (acceptBtn) {
            acceptBtn.addEventListener('click', function() {
                localStorage.setItem('cookieConsent', 'accepted');
                cookieConsent.hidden = true;
            });
        }

        if (declineBtn) {
            declineBtn.addEventListener('click', function() {
                localStorage.setItem('cookieConsent', 'declined');
                cookieConsent.hidden = true;
            });
        }
    }

    // AOS (Animate On Scroll)
    function initAOS() {
        if (typeof AOS !== 'undefined') {
            AOS.init({
                duration: 800,
                easing: 'ease-out-cubic',
                once: true,
                mirror: false,
                offset: 100
            });
        }
    }

    // Smooth Scroll for Anchor Links
    function initSmoothScroll() {
        document.querySelectorAll('a[href^="#"]').forEach(function(anchor) {
            anchor.addEventListener('click', function(e) {
                const targetId = this.getAttribute('href');
                if (targetId === '#') return;
                
                const target = document.querySelector(targetId);
                if (target) {
                    e.preventDefault();
                    const headerOffset = 80;
                    const elementPosition = target.getBoundingClientRect().top;
                    const offsetPosition = elementPosition + window.scrollY - headerOffset;

                    window.scrollTo({
                        top: offsetPosition,
                        behavior: 'smooth'
                    });

                    // Focus for accessibility
                    target.focus({ preventScroll: true });
                }
            });
        });
    }

    // Lazy Loading for Images
    function initLazyLoading() {
        if ('IntersectionObserver' in window) {
            const imageObserver = new IntersectionObserver(function(entries, observer) {
                entries.forEach(function(entry) {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        if (img.dataset.src) {
                            img.src = img.dataset.src;
                            img.removeAttribute('data-src');
                        }
                        if (img.dataset.srcset) {
                            img.srcset = img.dataset.srcset;
                            img.removeAttribute('data-srcset');
                        }
                        img.classList.add('loaded');
                        observer.unobserve(img);
                    }
                });
            }, {
                rootMargin: '50px 0px',
                threshold: 0.01
            });

            document.querySelectorAll('img[data-src], img[data-srcset]').forEach(function(img) {
                imageObserver.observe(img);
            });
        }
    }

    // Form Handling
    function initForms() {
        // AJAX Form Submission
        document.querySelectorAll('form[data-ajax="true"]').forEach(function(form) {
            form.addEventListener('submit', function(e) {
                e.preventDefault();
                
                const submitBtn = form.querySelector('button[type="submit"]');
                const originalText = submitBtn ? submitBtn.innerHTML : '';
                
                // Show loading state
                if (submitBtn) {
                    submitBtn.disabled = true;
                    submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Submitting...';
                }

                const formData = new FormData(form);
                const action = form.action || window.location.href;
                const method = form.method || 'POST';

                fetch(action, {
                    method: method,
                    body: formData,
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                    }
                })
                .then(function(response) {
                    return response.json();
                })
                .then(function(data) {
                    if (data.success) {
                        showToast(data.message || 'Form submitted successfully', 'success');
                        form.reset();
                        // Reset captcha if exists
                        if (typeof grecaptcha !== 'undefined') {
                            grecaptcha.reset();
                        }
                    } else {
                        showToast(data.message || 'An error occurred', 'error');
                        if (data.errors) {
                            displayFormErrors(form, data.errors);
                        }
                    }
                })
                .catch(function(error) {
                    console.error('Form submission error:', error);
                    showToast('An error occurred. Please try again.', 'error');
                })
                .finally(function() {
                    if (submitBtn) {
                        submitBtn.disabled = false;
                        submitBtn.innerHTML = originalText;
                    }
                });
            });
        });

        // Real-time validation
        document.querySelectorAll('form').forEach(function(form) {
            form.querySelectorAll('input, textarea, select').forEach(function(input) {
                input.addEventListener('blur', function() {
                    validateField(this);
                });
                
                input.addEventListener('input', function() {
                    if (this.classList.contains('is-invalid')) {
                        validateField(this);
                    }
                });
            });
        });
    }

    function validateField(field) {
        const value = field.value.trim();
        let isValid = true;
        let message = '';

        // Required
        if (field.hasAttribute('required') && !value) {
            isValid = false;
            message = field.getAttribute('data-required-message') || 'This field is required';
        }

        // Email
        if (isValid && field.type === 'email' && value) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(value)) {
                isValid = false;
                message = field.getAttribute('data-email-message') || 'Please enter a valid email address';
            }
        }

        // Phone
        if (isValid && field.type === 'tel' && value) {
            const phoneRegex = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/;
            if (!phoneRegex.test(value)) {
                isValid = false;
                message = field.getAttribute('data-phone-message') || 'Please enter a valid phone number';
            }
        }

        // Min length
        if (isValid && field.hasAttribute('minlength') && value.length < parseInt(field.getAttribute('minlength'))) {
            isValid = false;
            message = field.getAttribute('data-minlength-message') || `Minimum ${field.getAttribute('minlength')} characters required`;
        }

        // Max length
        if (isValid && field.hasAttribute('maxlength') && value.length > parseInt(field.getAttribute('maxlength'))) {
            isValid = false;
            message = field.getAttribute('data-maxlength-message') || `Maximum ${field.getAttribute('maxlength')} characters allowed`;
        }

        // Pattern
        if (isValid && field.hasAttribute('pattern') && value) {
            const pattern = new RegExp(field.getAttribute('pattern'));
            if (!pattern.test(value)) {
                isValid = false;
                message = field.getAttribute('data-pattern-message') || 'Invalid format';
            }
        }

        // Update UI
        if (isValid) {
            field.classList.remove('is-invalid');
            field.classList.add('is-valid');
            removeFieldError(field);
        } else {
            field.classList.remove('is-valid');
            field.classList.add('is-invalid');
            showFieldError(field, message);
        }

        return isValid;
    }

    function showFieldError(field, message) {
        removeFieldError(field);
        const errorDiv = document.createElement('div');
        errorDiv.className = 'invalid-feedback';
        errorDiv.textContent = message;
        field.parentNode.appendChild(errorDiv);
    }

    function removeFieldError(field) {
        const existingError = field.parentNode.querySelector('.invalid-feedback');
        if (existingError) {
            existingError.remove();
        }
    }

    function displayFormErrors(form, errors) {
        Object.keys(errors).forEach(function(fieldName) {
            const field = form.querySelector('[name="' + fieldName + '"]');
            if (field) {
                showFieldError(field, errors[fieldName]);
                field.classList.add('is-invalid');
            }
        });
    }

    // Counter Animation
    function initCounters() {
        const counters = document.querySelectorAll('[data-count]');
        if (!counters.length) return;

        const counterObserver = new IntersectionObserver(function(entries) {
            entries.forEach(function(entry) {
                if (entry.isIntersecting) {
                    const counter = entry.target;
                    const target = parseInt(counter.getAttribute('data-count'));
                    const duration = parseInt(counter.getAttribute('data-duration')) || 2000;
                    const step = Math.ceil(target / (duration / 16));
                    let current = 0;

                    const timer = setInterval(function() {
                        current += step;
                        if (current >= target) {
                            current = target;
                            clearInterval(timer);
                        }
                        counter.textContent = formatNumber(current);
                    }, 16);

                    counterObserver.unobserve(counter);
                }
            });
        }, { threshold: 0.5 });

        counters.forEach(function(counter) {
            counterObserver.observe(counter);
        });
    }

    function formatNumber(num) {
        return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    }

    // Bootstrap Tooltips
    function initTooltips() {
        if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
            const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
            tooltipTriggerList.map(function(tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl);
            });
        }
    }

    // Bootstrap Dropdowns
    function initDropdowns() {
        if (typeof bootstrap !== 'undefined' && bootstrap.Dropdown) {
            const dropdownElementList = [].slice.call(document.querySelectorAll('.dropdown-toggle'));
            dropdownElementList.map(function(dropdownToggleEl) {
                return new bootstrap.Dropdown(dropdownToggleEl);
            });
        }
    }

    // Toast Notifications
    window.showToast = function(message, type = 'info') {
        const toastContainer = getOrCreateToastContainer();
        
        const toast = document.createElement('div');
        toast.className = `toast align-items-center text-white bg-${type === 'success' ? 'success' : type === 'error' ? 'danger' : type === 'warning' ? 'warning' : 'primary'} border-0`;
        toast.setAttribute('role', 'alert');
        toast.setAttribute('aria-live', 'assertive');
        toast.setAttribute('aria-atomic', 'true');
        
        toast.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">${message}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        `;
        
        toastContainer.appendChild(toast);
        
        const bsToast = new bootstrap.Toast(toast, {
            autohide: true,
            delay: 5000
        });
        
        bsToast.show();
        
        toast.addEventListener('hidden.bs.toast', function() {
            toast.remove();
        });
    };

    function getOrCreateToastContainer() {
        let container = document.getElementById('toast-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toast-container';
            container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
            container.style.zIndex = '9999';
            document.body.appendChild(container);
        }
        return container;
    }

    // Debounce Helper
    window.debounce = function(func, wait, immediate) {
        let timeout;
        return function() {
            const context = this;
            const args = arguments;
            const later = function() {
                timeout = null;
                if (!immediate) func.apply(context, args);
            };
            const callNow = immediate && !timeout;
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
            if (callNow) func.apply(context, args);
        };
    };

    // Throttle Helper
    window.throttle = function(func, limit) {
        let inThrottle;
        return function() {
            const args = arguments;
            const context = this;
            if (!inThrottle) {
                func.apply(context, args);
                inThrottle = true;
                setTimeout(function() {
                    inThrottle = false;
                }, limit);
            }
        };
    };

    // RTL Detection
    window.isRTL = function() {
        return document.dir === 'rtl' || document.documentElement.getAttribute('dir') === 'rtl';
    };

    // Language Switcher Enhancement
    document.querySelectorAll('.language-switcher a[href]').forEach(function(link) {
        link.addEventListener('click', function(e) {
            // Allow default behavior for language switching
            // The server will handle the culture cookie
        });
    });

    // Mobile Menu Toggle Enhancement
    const navbarToggler = document.querySelector('.navbar-toggler');
    const navbarCollapse = document.querySelector('.navbar-collapse');
    
    if (navbarToggler && navbarCollapse) {
        navbarToggler.addEventListener('click', function() {
            const isExpanded = this.getAttribute('aria-expanded') === 'true';
            this.setAttribute('aria-expanded', !isExpanded);
        });
    }

    // Dropdown Hover for Desktop
    if (window.innerWidth > 991) {
        document.querySelectorAll('.dropdown').forEach(function(dropdown) {
            dropdown.addEventListener('mouseenter', function() {
                const toggle = this.querySelector('.dropdown-toggle');
                if (toggle && !toggle.classList.contains('show')) {
                    toggle.click();
                }
            });
            
            dropdown.addEventListener('mouseleave', function() {
                const toggle = this.querySelector('.dropdown-toggle');
                if (toggle && toggle.classList.contains('show')) {
                    toggle.click();
                }
            });
        });
    }

    // Image Error Handling
    document.querySelectorAll('img').forEach(function(img) {
        img.addEventListener('error', function() {
            if (!this.dataset.fallbackApplied) {
                this.dataset.fallbackApplied = 'true';
                this.src = 'https://via.placeholder.com/400x300/e2e8f0/64748b?text=Image+Not+Found';
                this.alt = 'Image not available';
            }
        });
    });

    // Keyboard Navigation for Cards
    document.querySelectorAll('.service-card, .project-card, .news-card').forEach(function(card) {
        card.addEventListener('keydown', function(e) {
            if (e.key === 'Enter' || e.key === ' ') {
                const link = this.querySelector('a');
                if (link) {
                    e.preventDefault();
                    link.click();
                }
            }
        });
    });

    // Reading Progress Bar
    function initReadingProgress() {
        const progressBar = document.createElement('div');
        progressBar.id = 'reading-progress';
        progressBar.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            height: 3px;
            background: var(--primary-color);
            z-index: 9999;
            transform: scaleX(0);
            transform-origin: left;
            transition: transform 0.1s linear;
        `;
        document.body.appendChild(progressBar);

        window.addEventListener('scroll', function() {
            const scrollTop = window.scrollY;
            const docHeight = document.documentElement.scrollHeight - window.innerHeight;
            const scrollPercent = scrollTop / docHeight;
            progressBar.style.transform = `scaleX(${scrollPercent})`;
        });
    }

    // Initialize reading progress on article pages
    if (document.querySelector('article') || document.querySelector('.news-card')) {
        initReadingProgress();
    }

    // Performance: Preload next page on hover
    document.querySelectorAll('a[href^="/"]').forEach(function(link) {
        if (link.href && link.href.startsWith(window.location.origin)) {
            link.addEventListener('mouseenter', function() {
                if (!this.dataset.prefetched) {
                    this.dataset.prefetched = 'true';
                    const linkRel = document.createElement('link');
                    linkRel.rel = 'prefetch';
                    linkRel.href = this.href;
                    document.head.appendChild(linkRel);
                }
            }, { once: true });
        }
    });

    // Service Worker Registration (for PWA)
    if ('serviceWorker' in navigator) {
        window.addEventListener('load', function() {
            navigator.serviceWorker.register('/sw.js').catch(function(error) {
                console.log('Service Worker registration failed:', error);
            });
        });
    }

})();
