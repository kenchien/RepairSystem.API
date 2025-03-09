(function umd(root, factory) {
  if (typeof module === 'object' && typeof exports === 'object')
    module.exports = factory();
  else if (typeof define === 'function' && define.amd)
    define([], factory);
  else
    root.httpVueLoader = factory();
})(this, function factory() {
  'use strict';

  var scopeIndex = 0;

  StyleContext.prototype = {
    withBase: function(callback) {
      var baseUrl = this.baseUrl;
      if (baseUrl === null) {
        baseUrl = window.location.toString().split('#')[0].split('?')[0].split('/').slice(0, -1).join('/');
      }
      return callback(baseUrl);
    },
    load: function(templateEl) {
      if (this.component) this.component.$el = templateEl;
    }
  };

  function StyleContext(component, baseUrl) {
    this.component = component;
    this.baseUrl = baseUrl;
  }

  httpVueLoader.parseComponentURL = function(url) {
    var comp = url.match(/(.*?)([^/]+?)\/?(\.vue)?(\?.*|#.*|$)/);
    return {
      name: comp[2],
      url: comp[1] + comp[2] + (comp[3] === undefined ? '/index.vue' : comp[3]) + comp[4]
    };
  };

  httpVueLoader.scopeStyles = function(styleEl, scopeName) {
    function process() {
      var sheet = styleEl.sheet;
      var rules = sheet.cssRules;
      for (var i = 0; i < rules.length; ++i) {
        var rule = rules[i];
        if (rule.type !== 1)
          continue;
        var scopedSelectors = [];
        rule.selectorText.split(/\s*,\s*/).forEach(function(sel) {
          scopedSelectors.push(sel.replace(/^(\s*)/, '$1[' + scopeName + ']'));
        });
        var scopedRule = scopedSelectors.join(',') + rule.cssText.substr(rule.selectorText.length);
        sheet.deleteRule(i);
        sheet.insertRule(scopedRule, i);
      }
    }
    try {
      process();
    } catch (ex) {
      styleEl.sheet.disabled = true;
      styleEl.addEventListener('load', function() {
        process();
        styleEl.sheet.disabled = false;
      });
    }
  };

  httpVueLoader.scopeIndex = 0;

  function httpVueLoader(url, name) {
    var comp = httpVueLoader.parseComponentURL(url);
    return httpVueComponentLoader(comp.url, name);
  }

  function httpVueComponentLoader(url, name) {
    return function(resolve, reject) {
      function getContent(url) {
        return new Promise(function(resolve, reject) {
          var xhr = new XMLHttpRequest();
          xhr.open('GET', url);
          xhr.responseType = 'text';
          xhr.onreadystatechange = function() {
            if (xhr.readyState === 4) {
              if (xhr.status >= 200 && xhr.status < 300)
                resolve(xhr.responseText);
              else
                reject(xhr.status);
            }
          };
          xhr.send(null);
        });
      }

      getContent(url)
      .then(function(content) {
        var templateMatch = content.match(/<template>([\s\S]+?)<\/template>/i);
        var scriptMatch = content.match(/<script>([\s\S]+?)<\/script>/i);
        var stylesMatch = content.match(/<style[^>]*>([\s\S]+?)<\/style>/gi);

        // 解析模板
        var template = templateMatch ? templateMatch[1] : '';
        
        // 解析腳本
        var script = '';
        if (scriptMatch) {
          script = scriptMatch[1];
        }

        var component = { template: template };
        var baseUrl = url.substring(0, url.lastIndexOf('/') + 1);

        // 確保name存在
        component.name = name || component.name || url.match(/([^/]+?)\.vue$/)[1];

        // 解析樣式
        if (stylesMatch) {
          var styleData = [];
          for (var i = 0; i < stylesMatch.length; ++i) {
            var styleMatch = stylesMatch[i].match(/<style([^>]*)>([\s\S]+?)<\/style>/i);
            var isScoped = styleMatch[1].indexOf('scoped') !== -1;
            var styleContent = styleMatch[2];
            styleData.push({ content: styleContent, scoped: isScoped });
          }
          
          // 添加樣式到頁面
          var scopeId = 'data-v-' + (name || 'scope-' + httpVueLoader.scopeIndex++);
          
          var styleEls = [];
          for (var i = 0; i < styleData.length; ++i) {
            var styleEl = document.createElement('style');
            styleEl.innerHTML = styleData[i].content;
            
            if (styleData[i].scoped) {
              styleEl.setAttribute('scoped', 'true');
              httpVueLoader.scopeStyles(styleEl, scopeId);
            }
            
            document.head.appendChild(styleEl);
            styleEls.push(styleEl);
          }
          
          // 為組件添加scopeId
          component._scopeId = scopeId;
        }

        // 使用eval解析腳本並應用到組件
        if (script) {
          var scriptContent = '(function() { ' + script + ' })()';
          var componentScript = eval(scriptContent);
          
          // 合併解析的腳本與組件
          if (componentScript.__esModule) componentScript = componentScript.default;
          for (var key in componentScript) {
            if (key === 'template') continue;
            component[key] = componentScript[key];
          }
        }

        resolve(component);
      })
      .catch(function(error) {
        reject(error);
      });
    };
  }

  return httpVueLoader;
}); 