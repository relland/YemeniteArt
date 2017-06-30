/*
* Kendo UI Web v2014.1.318 (http://kendoui.com)
* Copyright 2014 Telerik AD. All rights reserved.
*
* Kendo UI Web commercial licenses may be obtained at
* http://www.telerik.com/purchase/license-agreement/kendo-ui-web
* If you do not own a commercial license, this file shall be governed by the
* GNU General Public License (GPL) version 3.
* For GPL requirements, please review: http://www.gnu.org/copyleft/gpl.html
*/
!function(e,define){define(["./kendo.draganddrop.min"],e)}(function(){return function(e,t){function n(e,t,n){var i=n?" k-slider-horizontal":" k-slider-vertical",r=e.style?e.style:t.attr("style"),a=t.attr("class")?" "+t.attr("class"):"",o="";return"bottomRight"==e.tickPlacement?o=" k-slider-bottomright":"topLeft"==e.tickPlacement&&(o=" k-slider-topleft"),r=r?" style='"+r+"'":"","<div class='k-widget k-slider"+i+a+"'"+r+"><div class='k-slider-wrap"+(e.showButtons?" k-slider-buttons":"")+o+"'></div></div>"}function i(e,t,n){var i="";return i="increase"==t?n?"k-i-arrow-e":"k-i-arrow-n":n?"k-i-arrow-w":"k-i-arrow-s","<a class='k-button k-button-"+t+"'><span class='k-icon "+i+"' title='"+e[t+"ButtonTitle"]+"'>"+e[t+"ButtonTitle"]+"</span></a>"}function r(e,t){var n,i="<ul class='k-reset k-slider-items'>",r=b.floor(u(t/e.smallStep))+1;for(n=0;r>n;n++)i+="<li class='k-tick' role='presentation'>&nbsp;</li>";return i+="</ul>"}function a(e,t){var n=t.is("input")?1:2,i=2==n?e.leftDragHandleTitle:e.dragHandleTitle;return"<div class='k-slider-track'><div class='k-slider-selection'><!-- --></div><a href='#' class='k-draghandle' title='"+i+"' role='slider' aria-valuemin='"+e.min+"' aria-valuemax='"+e.max+"' aria-valuenow='"+(n>1?e.selectionStart||e.min:e.value||e.min)+"'>Drag</a>"+(n>1?"<a href='#' class='k-draghandle' title='"+e.rightDragHandleTitle+"'role='slider' aria-valuemin='"+e.min+"' aria-valuemax='"+e.max+"' aria-valuenow='"+(e.selectionEnd||e.max)+"'>Drag</a>":"")+"</div>"}function o(e){return function(t){return t+e}}function s(e){return function(){return e}}function l(e){return(e+"").replace(".",p.cultures.current.numberFormat["."])}function u(e){e=parseFloat(e,10);var t=b.pow(10,U||0);return b.round(e*t)/t}function c(e,n){var i=_(e.getAttribute(n));return null===i&&(i=t),i}function d(e){return typeof e!==$}var f,p=window.kendo,h=p.ui.Widget,g=p.ui.Draggable,m=e.extend,v=p.format,_=p.parseFloat,y=e.proxy,w=e.isArray,b=Math,x=p.support,k=x.pointers,C=x.msPointers,T="change",S="slide",D=".slider",F="touchstart"+D+" mousedown"+D,E=k?"pointerdown"+D:C?"MSPointerDown"+D:F,M="touchend"+D+" mouseup"+D,z=k?"pointerup":C?"MSPointerUp"+D:M,A="moveSelection",O="keydown"+D,H="click"+D,I="mouseover"+D,P="focus"+D,N="blur"+D,L=".k-draghandle",R=".k-slider-track",B=".k-tick",W="k-state-selected",q="k-state-focused",V="k-state-default",j="k-state-disabled",U=3,Y="disabled",$="undefined",G="tabindex",Q=p.getTouches,K=h.extend({init:function(e,t){var n,i=this;h.fn.init.call(i,e,t),t=i.options,i._distance=t.max-t.min,i._isHorizontal="horizontal"==t.orientation,i._isRtl=i._isHorizontal&&p.support.isRtl(e),i._position=i._isHorizontal?"left":"bottom",i._sizeFn=i._isHorizontal?"width":"height",i._outerSize=i._isHorizontal?"outerWidth":"outerHeight",t.tooltip.format=t.tooltip.enabled?t.tooltip.format||"{0}":"{0}",i._createHtml(),i.wrapper=i.element.closest(".k-slider"),i._trackDiv=i.wrapper.find(R),i._setTrackDivWidth(),i._maxSelection=i._trackDiv[i._sizeFn](),i._sliderItemsInit(),i._tabindex(i.wrapper.find(L)),i[t.enabled?"enable":"disable"](),n=p.support.isRtl(i.wrapper)?-1:1,i._keyMap={37:o(-1*n*t.smallStep),40:o(-t.smallStep),39:o(1*n*t.smallStep),38:o(+t.smallStep),35:s(t.max),36:s(t.min),33:o(+t.largeStep),34:o(-t.largeStep)},p.notify(i)},events:[T,S],options:{enabled:!0,min:0,max:10,smallStep:1,largeStep:5,orientation:"horizontal",tickPlacement:"both",tooltip:{enabled:!0,format:"{0}"}},_resize:function(){this._setTrackDivWidth(),this.wrapper.find(".k-slider-items").remove(),this._maxSelection=this._trackDiv[this._sizeFn](),this._sliderItemsInit(),this._refresh()},_sliderItemsInit:function(){var e=this,t=e.options,n=e._maxSelection/((t.max-t.min)/t.smallStep),i=e._calculateItemsWidth(b.floor(e._distance/t.smallStep));"none"!=t.tickPlacement&&n>=2&&(e._trackDiv.before(r(t,e._distance)),e._setItemsWidth(i),e._setItemsTitle()),e._calculateSteps(i),"none"!=t.tickPlacement&&n>=2&&t.largeStep>=t.smallStep&&e._setItemsLargeTick()},getSize:function(){return p.dimensions(this.wrapper)},_setTrackDivWidth:function(){var e=this,t=2*parseFloat(e._trackDiv.css(e._isRtl?"right":e._position),10);e._trackDiv[e._sizeFn](e.wrapper[e._sizeFn]()-2-t)},_setItemsWidth:function(t){var n,i=this,r=i.options,a=0,o=t.length-1,s=i.wrapper.find(B),l=0,u=2,c=s.length,d=0;for(n=0;c-2>n;n++)e(s[n+1])[i._sizeFn](t[n]);if(i._isHorizontal?(e(s[a]).addClass("k-first")[i._sizeFn](t[o-1]),e(s[o]).addClass("k-last")[i._sizeFn](t[o])):(e(s[o]).addClass("k-first")[i._sizeFn](t[o]),e(s[a]).addClass("k-last")[i._sizeFn](t[o-1])),i._distance%r.smallStep!==0&&!i._isHorizontal){for(n=0;t.length>n;n++)d+=t[n];l=i._maxSelection-d,l+=parseFloat(i._trackDiv.css(i._position),10)+u,i.wrapper.find(".k-slider-items").css("padding-top",l)}},_setItemsTitle:function(){for(var t=this,n=t.options,i=t.wrapper.find(B),r=n.min,a=i.length,o=t._isHorizontal&&!t._isRtl?0:a-1,s=t._isHorizontal&&!t._isRtl?a:-1,l=t._isHorizontal&&!t._isRtl?1:-1;o-s!==0;o+=l)e(i[o]).attr("title",v(n.tooltip.format,u(r))),r+=n.smallStep},_setItemsLargeTick:function(){var t,n,i=this,r=i.options,a=i.wrapper.find(B),o=0;if(1e3*r.largeStep%(1e3*r.smallStep)===0||i._distance/r.largeStep>=3)for(i._isHorizontal&&!i._isRtl&&(a=e.makeArray(a).reverse()),o=0;a.length>o;o++)t=e(a[o]),n=i._values[o],n%r.smallStep===0&&n%r.largeStep===0&&(t.addClass("k-tick-large").html("<span class='k-label'>"+t.attr("title")+"</span>"),0!==o&&o!==a.length-1&&t.css("line-height",t[i._sizeFn]()+"px"))},_calculateItemsWidth:function(e){var t,n,i,r=this,a=r.options,o=parseFloat(r._trackDiv.css(r._sizeFn))+1,s=o/r._distance;for(r._distance/a.smallStep-b.floor(r._distance/a.smallStep)>0&&(o-=r._distance%a.smallStep*s),t=o/e,n=[],i=0;e-1>i;i++)n[i]=t;return n[e-1]=n[e]=t/2,r._roundWidths(n)},_roundWidths:function(e){var t,n=0,i=e.length;for(t=0;i>t;t++)n+=e[t]-b.floor(e[t]),e[t]=b.floor(e[t]);return n=b.round(n),this._addAdditionalSize(n,e)},_addAdditionalSize:function(e,t){if(0===e)return t;var n,i=parseFloat(t.length-1)/parseFloat(1==e?e:e-1);for(n=0;e>n;n++)t[parseInt(b.round(i*n),10)]+=1;return t},_calculateSteps:function(e){var t,n=this,i=n.options,r=i.min,a=0,o=b.ceil(n._distance/i.smallStep),s=1;if(o+=n._distance/i.smallStep%1===0?1:0,e.splice(0,0,2*e[o-2]),e.splice(o-1,1,2*e.pop()),n._pixelSteps=[a],n._values=[r],0!==o){for(;o>s;)a+=(e[s-1]+e[s])/2,n._pixelSteps[s]=a,n._values[s]=r+=i.smallStep,s++;t=n._distance%i.smallStep===0?o-1:o,n._pixelSteps[t]=n._maxSelection,n._values[t]=i.max,n._isRtl&&(n._pixelSteps.reverse(),n._values.reverse())}},_getValueFromPosition:function(e,t){var n,i=this,r=i.options,a=b.max(r.smallStep*(i._maxSelection/i._distance),0),o=0,s=a/2;if(i._isHorizontal?(o=e-t.startPoint,i._isRtl&&(o=i._maxSelection-o)):o=t.startPoint-e,i._maxSelection-(parseInt(i._maxSelection%a,10)-3)/2<o)return r.max;for(n=0;i._pixelSteps.length>n;n++)if(b.abs(i._pixelSteps[n]-o)-1<=s)return u(i._values[n])},_getFormattedValue:function(e,t){var n,i,r,a=this,o="",s=a.options.tooltip;return w(e)?(i=e[0],r=e[1]):t&&t.type&&(i=t.selectionStart,r=t.selectionEnd),t&&(n=t.tooltipTemplate),!n&&s.template&&(n=p.template(s.template)),w(e)||t&&t.type?n?o=n({selectionStart:i,selectionEnd:r}):(i=v(s.format,i),r=v(s.format,r),o=i+" - "+r):(t&&(t.val=e),o=n?n({value:e}):v(s.format,e)),o},_getDraggableArea:function(){var e=this,t=p.getOffset(e._trackDiv);return{startPoint:e._isHorizontal?t.left:t.top+e._maxSelection,endPoint:e._isHorizontal?t.left+e._maxSelection:t.top}},_createHtml:function(){var e=this,t=e.element,r=e.options,o=t.find("input");2==o.length?(o.eq(0).val(r.selectionStart),o.eq(1).val(r.selectionEnd)):t.val(r.value),t.wrap(n(r,t,e._isHorizontal)).hide(),r.showButtons&&t.before(i(r,"increase",e._isHorizontal)).before(i(r,"decrease",e._isHorizontal)),t.before(a(r,t))},_focus:function(t){var n=this,i=t.target,r=n.value(),a=n._drag;a||(i==n.wrapper.find(L).eq(0)[0]?(a=n._firstHandleDrag,n._activeHandle=0):(a=n._lastHandleDrag,n._activeHandle=1),r=r[n._activeHandle]),e(i).addClass(q+" "+W),a&&(n._activeHandleDrag=a,a.selectionStart=n.options.selectionStart,a.selectionEnd=n.options.selectionEnd,a._updateTooltip(r))},_focusWithMouse:function(t){t=e(t);var n=this,i=t.is(L)?t.index():0;window.setTimeout(function(){n.wrapper.find(L)[2==i?1:0].focus()},1),n._setTooltipTimeout()},_blur:function(t){var n=this,i=n._activeHandleDrag;e(t.target).removeClass(q+" "+W),i&&(i._removeTooltip(),delete n._activeHandleDrag,delete n._activeHandle)},_setTooltipTimeout:function(){var e=this;e._tooltipTimeout=window.setTimeout(function(){var t=e._drag||e._activeHandleDrag;t&&t._removeTooltip()},300)},_clearTooltipTimeout:function(){var e,t=this;window.clearTimeout(this._tooltipTimeout),e=t._drag||t._activeHandleDrag,e&&e.tooltipDiv&&e.tooltipDiv.stop(!0,!1).css("opacity",1)}}),J=K.extend({init:function(n,i){var r,a=this;n.type="text",i=m({},{value:c(n,"value"),min:c(n,"min"),max:c(n,"max"),smallStep:c(n,"step")},i),n=e(n),i&&i.enabled===t&&(i.enabled=!n.is("[disabled]")),K.fn.init.call(a,n,i),i=a.options,d(i.value)&&null!==i.value||(i.value=i.min,n.val(i.min)),i.value=b.max(b.min(i.value,i.max),i.min),r=a.wrapper.find(L),new J.Selection(r,a,i),a._drag=new J.Drag(r,"",a,i)},options:{name:"Slider",showButtons:!0,increaseButtonTitle:"Increase",decreaseButtonTitle:"Decrease",dragHandleTitle:"drag",tooltip:{format:"{0:#,#.##}"},value:null},enable:function(n){var i,r,a,o=this,s=o.options;o.disable(),n!==!1&&(o.wrapper.removeClass(j).addClass(V),o.wrapper.find("input").removeAttr(Y),i=function(n){var i,r,a,s=Q(n)[0];if(s){if(i=o._isHorizontal?s.location.pageX:s.location.pageY,r=o._getDraggableArea(),a=e(n.target),a.hasClass("k-draghandle"))return a.addClass(q+" "+W),t;o._update(o._getValueFromPosition(i,r)),o._focusWithMouse(n.target),o._drag.dragstart(n),n.preventDefault()}},o.wrapper.find(B+", "+R).on(E,i).end().on(E,function(){e(document.documentElement).one("selectstart",p.preventDefault)}).on(z,function(){o._drag._end()}),o.wrapper.find(L).attr(G,0).on(M,function(){o._setTooltipTimeout()}).on(H,function(e){o._focusWithMouse(e.target),e.preventDefault()}).on(P,y(o._focus,o)).on(N,y(o._blur,o)),r=y(function(e){var t=o._nextValueByIndex(o._valueIndex+1*e);o._setValueInRange(t),o._drag._updateTooltip(t)},o),s.showButtons&&(a=y(function(e,t){this._clearTooltipTimeout(),(1===e.which||x.touch&&0===e.which)&&(r(t),this.timeout=setTimeout(y(function(){this.timer=setInterval(function(){r(t)},60)},this),200))},o),o.wrapper.find(".k-button").on(M,y(function(e){this._clearTimer(),o._focusWithMouse(e.target)},o)).on(I,function(t){e(t.currentTarget).addClass("k-state-hover")}).on("mouseout"+D,y(function(t){e(t.currentTarget).removeClass("k-state-hover"),this._clearTimer()},o)).eq(0).on(F,y(function(e){a(e,1)},o)).click(!1).end().eq(1).on(F,y(function(e){a(e,-1)},o)).click(p.preventDefault)),o.wrapper.find(L).off(O,!1).on(O,y(this._keydown,o)),s.enabled=!0)},disable:function(){var t=this;t.wrapper.removeClass(V).addClass(j),e(t.element).prop(Y,Y),t.wrapper.find(".k-button").off(F).on(F,p.preventDefault).off(M).on(M,p.preventDefault).off("mouseleave"+D).on("mouseleave"+D,p.preventDefault).off(I).on(I,p.preventDefault),t.wrapper.find(B+", "+R).off(E).off(z),t.wrapper.find(L).attr(G,-1).off(M).off(O).off(H).off(P).off(N),t.options.enabled=!1},_update:function(e){var t=this,n=t.value()!=e;t.value(e),n&&t.trigger(T,{value:t.options.value})},value:function(e){var n=this,i=n.options;return e=u(e),isNaN(e)?i.value:(e>=i.min&&i.max>=e&&i.value!=e&&(n.element.prop("value",l(e)),i.value=e,n._refreshAriaAttr(e),n._refresh()),t)},_refresh:function(){this.trigger(A,{value:this.options.value})},_refreshAriaAttr:function(e){var t,n=this,i=n._drag;t=i&&i._tooltipDiv?i._tooltipDiv.text():n._getFormattedValue(e,null),this.wrapper.find(L).attr("aria-valuenow",e).attr("aria-valuetext",t)},_clearTimer:function(){clearTimeout(this.timeout),clearInterval(this.timer)},_keydown:function(e){var t=this;e.keyCode in t._keyMap&&(t._clearTooltipTimeout(),t._setValueInRange(t._keyMap[e.keyCode](t.options.value)),t._drag._updateTooltip(t.value()),e.preventDefault())},_setValueInRange:function(e){var n=this,i=n.options;return e=u(e),isNaN(e)?(n._update(i.min),t):(e=b.max(b.min(e,i.max),i.min),n._update(e),t)},_nextValueByIndex:function(e){var t=this._values.length;return this._isRtl&&(e=t-1-e),this._values[b.max(0,b.min(e,t-1))]},destroy:function(){var e=this;h.fn.destroy.call(e),e.wrapper.off(D).find(".k-button").off(D).end().find(L).off(D).end().find(B+", "+R).off(D).end(),e._drag.draggable.destroy(),e._drag._removeTooltip(!0)}});J.Selection=function(e,t,n){function i(i){var r=i-n.min,a=t._valueIndex=b.ceil(u(r/n.smallStep)),o=parseInt(t._pixelSteps[a],10),s=t._trackDiv.find(".k-slider-selection"),l=parseInt(e[t._outerSize]()/2,10),c=t._isRtl?2:0;s[t._sizeFn](t._isRtl?t._maxSelection-o:o),e.css(t._position,o-l-c)}i(n.value),t.bind([T,S,A],function(e){i(parseFloat(e.value,10))})},J.Drag=function(e,t,n,i){var r=this;r.owner=n,r.options=i,r.element=e,r.type=t,r.draggable=new g(e,{distance:0,dragstart:y(r._dragstart,r),drag:y(r.drag,r),dragend:y(r.dragend,r),dragcancel:y(r.dragcancel,r)}),e.click(!1)},J.Drag.prototype={dragstart:function(e){this.owner._activeDragHandle=this,this.draggable.userEvents.cancel(),this.draggable.userEvents._start(e)},_dragstart:function(n){var i=this,r=i.owner,a=i.options;return a.enabled?(this.owner._activeDragHandle=this,r.element.off(I),i.element.addClass(q+" "+W),e(document.documentElement).css("cursor","pointer"),i.dragableArea=r._getDraggableArea(),i.step=b.max(a.smallStep*(r._maxSelection/r._distance),0),i.type?(i.selectionStart=a.selectionStart,i.selectionEnd=a.selectionEnd,r._setZIndex(i.type)):i.oldVal=i.val=a.value,i._removeTooltip(!0),i._createTooltip(),t):(n.preventDefault(),t)},_createTooltip:function(){var t,n,i=this,r=i.owner,a=i.options.tooltip,o="",s=e(window);a.enabled&&(a.template&&(t=i.tooltipTemplate=p.template(a.template)),e(".k-slider-tooltip").remove(),i.tooltipDiv=e("<div class='k-widget k-tooltip k-slider-tooltip'><!-- --></div>").appendTo(document.body),o=r._getFormattedValue(i.val||r.value(),i),i.type||(n="k-callout-"+(r._isHorizontal?"s":"e"),i.tooltipInnerDiv="<div class='k-callout "+n+"'><!-- --></div>",o+=i.tooltipInnerDiv),i.tooltipDiv.html(o),i._scrollOffset={top:s.scrollTop(),left:s.scrollLeft()},i.moveTooltip())},drag:function(e){var t,n=this,i=n.owner,r=e.x.location,a=e.y.location,o=n.dragableArea.startPoint,s=n.dragableArea.endPoint;e.preventDefault(),n.val=i._isHorizontal?i._isRtl?n.constrainValue(r,o,s,s>r):n.constrainValue(r,o,s,r>=s):n.constrainValue(a,s,o,s>=a),n.oldVal!=n.val&&(n.oldVal=n.val,n.type?("firstHandle"==n.type?n.selectionStart=n.selectionEnd>n.val?n.val:n.selectionEnd=n.val:n.val>n.selectionStart?n.selectionEnd=n.val:n.selectionStart=n.selectionEnd=n.val,t={values:[n.selectionStart,n.selectionEnd],value:[n.selectionStart,n.selectionEnd]}):t={value:n.val},i.trigger(S,t)),n._updateTooltip(n.val)},_updateTooltip:function(e){var t=this,n=t.options,i=n.tooltip,r="";i.enabled&&(t.tooltipDiv||t._createTooltip(),r=t.owner._getFormattedValue(u(e),t),t.type||(r+=t.tooltipInnerDiv),t.tooltipDiv.html(r),t.moveTooltip())},dragcancel:function(){return this.owner._refresh(),e(document.documentElement).css("cursor",""),this._end()},dragend:function(){var t=this,n=t.owner;return e(document.documentElement).css("cursor",""),t.type?n._update(t.selectionStart,t.selectionEnd):(n._update(t.val),t.draggable.userEvents._disposeAll()),t._end()},_end:function(){var e=this,t=e.owner;return t._focusWithMouse(e.element),t.element.on(I),!1},_removeTooltip:function(t){var n=this,i=n.owner;n.tooltipDiv&&i.options.tooltip.enabled&&i.options.enabled&&(t?(n.tooltipDiv.remove(),n.tooltipDiv=null):n.tooltipDiv.fadeOut("slow",function(){e(this).remove(),n.tooltipDiv=null}))},moveTooltip:function(){var t,n,i,r,a=this,o=a.owner,s=0,l=0,u=a.element,c=p.getOffset(u),d=8,f=e(window),h=a.tooltipDiv.find(".k-callout"),g=a.tooltipDiv.outerWidth(),m=a.tooltipDiv.outerHeight();a.type?(t=o.wrapper.find(L),c=p.getOffset(t.eq(0)),n=p.getOffset(t.eq(1)),o._isHorizontal?(s=n.top,l=c.left+(n.left-c.left)/2):(s=c.top+(n.top-c.top)/2,l=n.left),r=t.eq(0).outerWidth()+2*d):(s=c.top,l=c.left,r=u.outerWidth()+2*d),o._isHorizontal?(l-=parseInt((g-u[o._outerSize]())/2,10),s-=m+h.height()+d):(s-=parseInt((m-u[o._outerSize]())/2,10),l-=g+h.width()+d),o._isHorizontal?(i=a._flip(s,m,r,f.outerHeight()+a._scrollOffset.top),s+=i,l+=a._fit(l,g,f.outerWidth()+a._scrollOffset.left)):(i=a._flip(l,g,r,f.outerWidth()+a._scrollOffset.left),s+=a._fit(s,m,f.outerHeight()+a._scrollOffset.top),l+=i),i>0&&h&&(h.removeClass(),h.addClass("k-callout k-callout-"+(o._isHorizontal?"n":"w"))),a.tooltipDiv.css({top:s,left:l})},_fit:function(e,t,n){var i=0;return e+t>n&&(i=n-(e+t)),0>e&&(i=-e),i},_flip:function(e,t,n,i){var r=0;return e+t>i&&(r+=-(n+t)),0>e+r&&(r+=n+t),r},constrainValue:function(e,t,n,i){var r=this,a=0;return a=e>t&&n>e?r.owner._getValueFromPosition(e,r.dragableArea):i?r.options.max:r.options.min}},p.ui.plugin(J),f=K.extend({init:function(n,i){var r,a=this,o=e(n).find("input"),s=o.eq(0)[0],l=o.eq(1)[0];s.type="text",l.type="text",i=m({},{selectionStart:c(s,"value"),min:c(s,"min"),max:c(s,"max"),smallStep:c(s,"step")},{selectionEnd:c(l,"value"),min:c(l,"min"),max:c(l,"max"),smallStep:c(l,"step")},i),i&&i.enabled===t&&(i.enabled=!o.is("[disabled]")),K.fn.init.call(a,n,i),i=a.options,d(i.selectionStart)&&null!==i.selectionStart||(i.selectionStart=i.min,o.eq(0).val(i.min)),d(i.selectionEnd)&&null!==i.selectionEnd||(i.selectionEnd=i.max,o.eq(1).val(i.max)),r=a.wrapper.find(L),new f.Selection(r,a,i),a._firstHandleDrag=new J.Drag(r.eq(0),"firstHandle",a,i),a._lastHandleDrag=new J.Drag(r.eq(1),"lastHandle",a,i)},options:{name:"RangeSlider",leftDragHandleTitle:"drag",rightDragHandleTitle:"drag",tooltip:{format:"{0:#,#.##}"},selectionStart:null,selectionEnd:null},enable:function(n){var i,r=this,a=r.options;r.disable(),n!==!1&&(r.wrapper.removeClass(j).addClass(V),r.wrapper.find("input").removeAttr(Y),i=function(n){var i,o,s,l,u,c,d,f=Q(n)[0];if(f){if(i=r._isHorizontal?f.location.pageX:f.location.pageY,o=r._getDraggableArea(),s=r._getValueFromPosition(i,o),l=e(n.target),l.hasClass("k-draghandle"))return l.addClass(q+" "+W),t;a.selectionStart>s?(u=s,c=a.selectionEnd,d=r._firstHandleDrag):s>r.selectionEnd?(u=a.selectionStart,c=s,d=r._lastHandleDrag):a.selectionEnd-s>=s-a.selectionStart?(u=s,c=a.selectionEnd,d=r._firstHandleDrag):(u=a.selectionStart,c=s,d=r._lastHandleDrag),d.dragstart(n),r._setValueInRange(u,c),r._focusWithMouse(d.element)}},r.wrapper.find(B+", "+R).on(E,i).end().on(E,function(){e(document.documentElement).one("selectstart",p.preventDefault)}).on(z,function(){r._activeDragHandle&&r._activeDragHandle._end()}),r.wrapper.find(L).attr(G,0).on(M,function(){r._setTooltipTimeout()}).on(H,function(e){r._focusWithMouse(e.target),e.preventDefault()}).on(P,y(r._focus,r)).on(N,y(r._blur,r)),r.wrapper.find(L).off(O,p.preventDefault).eq(0).on(O,y(function(e){this._keydown(e,"firstHandle")},r)).end().eq(1).on(O,y(function(e){this._keydown(e,"lastHandle")},r)),r.options.enabled=!0)},disable:function(){var e=this;e.wrapper.removeClass(V).addClass(j),e.wrapper.find("input").prop(Y,Y),e.wrapper.find(B+", "+R).off(E).off(z),e.wrapper.find(L).attr(G,-1).off(M).off(O).off(H).off(P).off(N),e.options.enabled=!1},_keydown:function(e,t){var n,i,r,a=this,o=a.options.selectionStart,s=a.options.selectionEnd;e.keyCode in a._keyMap&&(a._clearTooltipTimeout(),"firstHandle"==t?(r=a._activeHandleDrag=a._firstHandleDrag,o=a._keyMap[e.keyCode](o),o>s&&(s=o)):(r=a._activeHandleDrag=a._lastHandleDrag,s=a._keyMap[e.keyCode](s),o>s&&(o=s)),a._setValueInRange(o,s),n=Math.max(o,a.options.selectionStart),i=Math.min(s,a.options.selectionEnd),r.selectionEnd=Math.max(i,a.options.selectionStart),r.selectionStart=Math.min(n,a.options.selectionEnd),r._updateTooltip(a.value()[a._activeHandle]),e.preventDefault())},_update:function(e,t){var n=this,i=n.value(),r=i[0]!=e||i[1]!=t;n.value([e,t]),r&&n.trigger(T,{values:[e,t],value:[e,t]})},value:function(e){return e&&e.length?this._value(e[0],e[1]):this._value()},_value:function(e,n){var i=this,r=i.options,a=r.selectionStart,o=r.selectionEnd;return isNaN(e)&&isNaN(n)?[a,o]:(e=u(e),n=u(n),e>=r.min&&r.max>=e&&n>=r.min&&r.max>=n&&n>=e&&(a!=e||o!=n)&&(i.element.find("input").eq(0).prop("value",l(e)).end().eq(1).prop("value",l(n)),r.selectionStart=e,r.selectionEnd=n,i._refresh(),i._refreshAriaAttr(e,n)),t)},values:function(e,t){return w(e)?this._value(e[0],e[1]):this._value(e,t)},_refresh:function(){var e=this,t=e.options;e.trigger(A,{values:[t.selectionStart,t.selectionEnd],value:[t.selectionStart,t.selectionEnd]}),t.selectionStart==t.max&&t.selectionEnd==t.max&&e._setZIndex("firstHandle")},_refreshAriaAttr:function(e,t){var n,i=this,r=i.wrapper.find(L),a=i._activeHandleDrag;n=i._getFormattedValue([e,t],a),r.eq(0).attr("aria-valuenow",e),r.eq(1).attr("aria-valuenow",t),r.attr("aria-valuetext",n)},_setValueInRange:function(e,t){var n=this.options;e=b.max(b.min(e,n.max),n.min),t=b.max(b.min(t,n.max),n.min),e==n.max&&t==n.max&&this._setZIndex("firstHandle"),this._update(b.min(e,t),b.max(e,t))},_setZIndex:function(t){this.wrapper.find(L).each(function(n){e(this).css("z-index","firstHandle"==t?1-n:n)})},destroy:function(){var e=this;h.fn.destroy.call(e),e.wrapper.off(D).find(B+", "+R).off(D).end().find(L).off(D),e._firstHandleDrag.draggable.destroy(),e._lastHandleDrag.draggable.destroy()}}),f.Selection=function(e,t,n){function i(i){i=i||[];var a=i[0]-n.min,o=i[1]-n.min,s=b.ceil(u(a/n.smallStep)),l=b.ceil(u(o/n.smallStep)),c=t._pixelSteps[s],d=t._pixelSteps[l],f=parseInt(e.eq(0)[t._outerSize]()/2,10),p=t._isRtl?2:0;e.eq(0).css(t._position,c-f-p).end().eq(1).css(t._position,d-f-p),r(c,d)}function r(e,n){var i,r,a=t._trackDiv.find(".k-slider-selection");i=b.abs(e-n),a[t._sizeFn](i),t._isRtl?(r=b.max(e,n),a.css("right",t._maxSelection-r-1)):(r=b.min(e,n),a.css(t._position,r-1))}i(t.value()),t.bind([T,S,A],function(e){i(e.values)})},p.ui.plugin(f)}(window.kendo.jQuery),window.kendo},"function"==typeof define&&define.amd?define:function(e,t){t()});