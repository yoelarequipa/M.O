/**
 * ══════════════════════════════════════════════════════════
 *  MÁRMOLES OESTE — script.js
 *  - Navbar sticky + scroll state
 *  - Hamburger menu toggle
 *  - Smooth scroll for anchor links
 *  - Scroll reveal animations (IntersectionObserver)
 *  - Product filter tabs
 *  - Contact form validation
 *  - Back-to-top button
 * ══════════════════════════════════════════════════════════
 */

'use strict';

/* ─────────────────────────────────────
   HELPERS
───────────────────────────────────── */

/**
 * Query selector shorthand
 * @param {string} sel - CSS selector
 * @param {Document|Element} [ctx=document]
 * @returns {Element|null}
 */
const $ = (sel, ctx = document) => ctx.querySelector(sel);

/**
 * Query selector all shorthand
 * @param {string} sel - CSS selector
 * @param {Document|Element} [ctx=document]
 * @returns {NodeList}
 */
const $$ = (sel, ctx = document) => ctx.querySelectorAll(sel);


/* ─────────────────────────────────────
   1. NAVBAR — STICKY + SCROLL STATE
───────────────────────────────────── */
(function initNavbar() {
  const navbar = $('#navbar');
  if (!navbar) return;

  const onScroll = () => {
    if (window.scrollY > 60) {
      navbar.classList.add('scrolled');
    } else {
      navbar.classList.remove('scrolled');
    }
  };

  // Passive listener for performance
  window.addEventListener('scroll', onScroll, { passive: true });

  // Run once on load (in case page loads mid-scroll)
  onScroll();
})();


/* ─────────────────────────────────────
   2. HAMBURGER MENU
───────────────────────────────────── */
(function initHamburger() {
  const hamburger = $('#hamburger');
  const navLinks  = $('#navLinks');
  if (!hamburger || !navLinks) return;

  /**
   * Open / close the mobile menu
   * @param {boolean} [force] - explicit state override
   */
  const toggleMenu = (force) => {
    const isOpen = typeof force === 'boolean'
      ? force
      : !hamburger.classList.contains('open');

    hamburger.classList.toggle('open', isOpen);
    navLinks.classList.toggle('open', isOpen);
    hamburger.setAttribute('aria-expanded', String(isOpen));

    // Prevent body scroll while menu is open
    document.body.style.overflow = isOpen ? 'hidden' : '';
  };

  // Toggle on hamburger click
  hamburger.addEventListener('click', () => toggleMenu());

  // Close when a nav link is clicked
  navLinks.addEventListener('click', (e) => {
    if (e.target.classList.contains('nav-link')) {
      toggleMenu(false);
    }
  });

  // Close on Escape key
  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && hamburger.classList.contains('open')) {
      toggleMenu(false);
    }
  });

  // Close when clicking outside the menu on mobile
  document.addEventListener('click', (e) => {
    if (
      hamburger.classList.contains('open') &&
      !navLinks.contains(e.target) &&
      !hamburger.contains(e.target)
    ) {
      toggleMenu(false);
    }
  });
})();


/* ─────────────────────────────────────
   3. SMOOTH SCROLL FOR ANCHOR LINKS
───────────────────────────────────── */
(function initSmoothScroll() {
  document.addEventListener('click', (e) => {
    const anchor = e.target.closest('a[href^="#"]');
    if (!anchor) return;

    const targetId = anchor.getAttribute('href');
    if (targetId === '#') return;

    const targetEl = document.querySelector(targetId);
    if (!targetEl) return;

    e.preventDefault();

    // Account for fixed navbar height
    const navbarHeight = $('#navbar')?.offsetHeight ?? 70;
    const targetY = targetEl.getBoundingClientRect().top + window.scrollY - navbarHeight;

    window.scrollTo({ top: targetY, behavior: 'smooth' });
  });
})();


/* ─────────────────────────────────────
   4. SCROLL REVEAL (IntersectionObserver)
───────────────────────────────────── */
(function initReveal() {
  const elements = $$('.reveal');
  if (!elements.length) return;

  // If browser doesn't support IntersectionObserver, just show all
  if (!('IntersectionObserver' in window)) {
    elements.forEach(el => el.classList.add('visible'));
    return;
  }

  const observer = new IntersectionObserver(
    (entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('visible');
          // Unobserve once visible to save resources
          observer.unobserve(entry.target);
        }
      });
    },
    {
      threshold: 0.12,       // Trigger when 12% of element is visible
      rootMargin: '0px 0px -40px 0px'  // Slightly before entering viewport
    }
  );

  elements.forEach(el => observer.observe(el));
})();


/* ─────────────────────────────────────
   5. PRODUCT FILTER TABS
───────────────────────────────────── */
(function initProductFilter() {
  const tabs  = $$('.filter-tab');
  const cards = $$('.product-card');
  if (!tabs.length || !cards.length) return;

  tabs.forEach(tab => {
    tab.addEventListener('click', () => {
      const filter = tab.dataset.filter;

      // Update active tab state
      tabs.forEach(t => {
        t.classList.remove('active');
        t.setAttribute('aria-selected', 'false');
      });
      tab.classList.add('active');
      tab.setAttribute('aria-selected', 'true');

      // Show / hide cards with a subtle animation
      cards.forEach(card => {
        const category = card.dataset.category;
        const shouldShow = filter === 'all' || category === filter;

        if (shouldShow) {
          card.classList.remove('product-card--hidden');
          // Re-trigger reveal animation
          card.classList.remove('visible');
          requestAnimationFrame(() => {
            card.classList.add('visible');
          });
        } else {
          card.classList.add('product-card--hidden');
        }
      });
    });
  });
})();


/* ─────────────────────────────────────
   6. CONTACT FORM VALIDATION
───────────────────────────────────── */
(function initContactForm() {
  const form    = $('#contactForm');
  const success = $('#formSuccess');
  if (!form) return;

  /**
   * Validate a single field
   * @param {HTMLInputElement|HTMLTextAreaElement} field
   * @returns {boolean}
   */
  const validateField = (field) => {
    const id       = field.id;
    const value    = field.value.trim();
    const errorEl  = $(`#error-${id}`);
    let   message  = '';

    switch (id) {
      case 'nombre':
        if (!value) {
          message = 'Por favor ingresá tu nombre.';
        } else if (value.length < 2) {
          message = 'El nombre debe tener al menos 2 caracteres.';
        }
        break;

      case 'telefono':
        if (!value) {
          message = 'Por favor ingresá tu teléfono.';
        } else if (!/^[\d\s\+\-\(\)]{7,20}$/.test(value)) {
          message = 'Ingresá un número de teléfono válido.';
        }
        break;

      case 'email':
        if (!value) {
          message = 'Por favor ingresá tu email.';
        } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
          message = 'Ingresá un email válido (ej: juan@mail.com).';
        }
        break;

      case 'mensaje':
        if (!value) {
          message = 'Por favor escribí tu mensaje.';
        } else if (value.length < 10) {
          message = 'El mensaje debe tener al menos 10 caracteres.';
        }
        break;
    }

    if (errorEl) errorEl.textContent = message;

    if (message) {
      field.classList.add('error');
      return false;
    } else {
      field.classList.remove('error');
      return true;
    }
  };

  // Real-time validation on blur
  $$('input, textarea', form).forEach(field => {
    field.addEventListener('blur', () => validateField(field));
    // Also clear error as user types
    field.addEventListener('input', () => {
      if (field.classList.contains('error')) validateField(field);
    });
  });

  // Form submit
  form.addEventListener('submit', (e) => {
    e.preventDefault();

    const fields  = $$('input, textarea', form);
    let   isValid = true;

    fields.forEach(field => {
      if (!validateField(field)) isValid = false;
    });

    if (!isValid) return;

    // ─── Simulated successful send ───
    // In production, replace this block with a fetch() to your backend/API.
    const submitBtn = $('button[type="submit"]', form);
    submitBtn.textContent = 'Enviando…';
    submitBtn.disabled    = true;

    setTimeout(() => {
      // Reset form
      form.reset();
      fields.forEach(field => field.classList.remove('error'));
      $$('[id^="error-"]', form).forEach(el => (el.textContent = ''));

      // Show success
      success.classList.add('show');

      // Re-enable button
      submitBtn.textContent = 'Enviar mensaje';
      submitBtn.disabled    = false;

      // Hide success message after 6 seconds
      setTimeout(() => success.classList.remove('show'), 6000);
    }, 1200);
  });
})();


/* ─────────────────────────────────────
   7. BACK TO TOP BUTTON
───────────────────────────────────── */
(function initBackToTop() {
  const btn = $('#backToTop');
  if (!btn) return;

  window.addEventListener(
    'scroll',
    () => {
      btn.classList.toggle('visible', window.scrollY > 500);
    },
    { passive: true }
  );

  btn.addEventListener('click', () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  });
})();


/* ─────────────────────────────────────
   8. HERO PARALLAX (desktop only)
   Moves hero content slightly on scroll
   for a subtle depth effect
───────────────────────────────────── */
(function initHeroParallax() {
  const heroContent = $('.hero__content');
  if (!heroContent) return;

  // Only on desktop (no parallax on touch devices for performance)
  const mq = window.matchMedia('(min-width: 769px) and (prefers-reduced-motion: no-preference)');

  if (!mq.matches) return;

  window.addEventListener(
    'scroll',
    () => {
      const scrollY = window.scrollY;
      if (scrollY < window.innerHeight) {
        heroContent.style.transform = `translateY(${scrollY * 0.22}px)`;
        heroContent.style.opacity   = 1 - scrollY / (window.innerHeight * 0.9);
      }
    },
    { passive: true }
  );
})();


/* ─────────────────────────────────────
   9. ACTIVE NAV LINK on scroll
   Highlights the nav link matching
   the currently visible section
───────────────────────────────────── */
(function initActiveNav() {
  const sections = $$('section[id]');
  const navLinks = $$('.nav-link');
  if (!sections.length || !navLinks.length) return;

  const navHeight = $('#navbar')?.offsetHeight ?? 70;

  const setActive = () => {
    let current = '';

    sections.forEach(section => {
      const sectionTop = section.offsetTop - navHeight - 100;
      if (window.scrollY >= sectionTop) {
        current = section.getAttribute('id');
      }
    });

    navLinks.forEach(link => {
      link.classList.toggle(
        'active-link',
        link.getAttribute('href') === `#${current}`
      );
    });
  };

  window.addEventListener('scroll', setActive, { passive: true });
  setActive();
})();
