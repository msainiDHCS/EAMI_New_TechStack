﻿/**
 * [Chart.PieceLabel.js]{@link https://github.com/emn178/Chart.PieceLabel.js}
 *
 * @version 0.9.0
 * @author Chen, Yi-Cyuan [emn178@gmail.com]
 * @copyright Chen, Yi-Cyuan 2017
 * @license MIT
 */
(function () {
    function c() { this.drawDataset = this.drawDataset.bind(this) } "undefined" === typeof Chart ? console.warn("Can not find Chart object.") : (c.prototype.beforeDatasetsUpdate = function (a) { if (this.parseOptions(a) && "outside" === this.position) { var b = 1.5 * this.fontSize + 2; a.chartArea.top += b; a.chartArea.bottom -= b } }, c.prototype.afterDatasetsDraw = function (a) { this.parseOptions(a) && (this.labelBounds = [], a.config.data.datasets.forEach(this.drawDataset)) }, c.prototype.drawDataset = function (a) {
        for (var b = this.ctx, p = this.chartInstance,
        l = a._meta[Object.keys(a._meta)[0]], h = 0, f = 0; f < l.data.length; f++) {
            var g = l.data[f], d = g._view; if (0 !== d.circumference || this.showZero) {
                switch (this.render) {
                    case "value": var e = a.data[f]; this.format && (e = this.format(e)); e = e.toString(); break; case "label": e = p.config.data.labels[f]; break; case "image": e = this.images[f] ? this.loadImage(this.images[f]) : ""; break; default: var q = d.circumference / this.options.circumference * 100; q = parseFloat(q.toFixed(this.precision)); this.showActualPercentages || (h += q, 100 < h && (q -= h - 100, q = parseFloat(q.toFixed(this.precision))));
                        e = q + "%"
                } "function" === typeof this.render && (e = this.render({ label: p.config.data.labels[f], value: a.data[f], percentage: q, dataset: a, index: f }), "object" === typeof e && (e = this.loadImage(e))); if (!e) break; b.save(); b.beginPath(); b.font = Chart.helpers.fontString(this.fontSize, this.fontStyle, this.fontFamily); if ("outside" === this.position || "border" === this.position && "pie" === p.config.type) {
                    var k = d.outerRadius / 2; var c, m = this.fontSize + 2; var n = d.startAngle + (d.endAngle - d.startAngle) / 2; "border" === this.position ? c = (d.outerRadius -
                    k) / 2 + k : "outside" === this.position && (c = d.outerRadius - k + k + m); n = { x: d.x + Math.cos(n) * c, y: d.y + Math.sin(n) * c }; if ("outside" === this.position) { n.x = n.x < d.x ? n.x - m : n.x + m; var r = d.outerRadius + m }
                } else k = d.innerRadius, n = g.tooltipPosition(); m = this.fontColor; "function" === typeof m ? m = m({ label: p.config.data.labels[f], value: a.data[f], percentage: q, text: e, backgroundColor: a.backgroundColor[f], dataset: a, index: f }) : "string" !== typeof m && (m = m[f] || this.options.defaultFontColor); if (this.arc) r || (r = (k + d.outerRadius) / 2), b.fillStyle =
                m, b.textBaseline = "middle", this.drawArcText(e, r, d, this.overlap); else { k = this.measureText(e); d = n.x - k.width / 2; k = n.x + k.width / 2; var t = n.y - this.fontSize / 2, u = n.y + this.fontSize / 2; (this.overlap || ("outside" === this.position ? this.checkTextBound(d, k, t, u) : g.inRange(d, t) && g.inRange(d, u) && g.inRange(k, t) && g.inRange(k, u))) && this.fillText(e, n, m) } b.restore()
            }
        }
    }, c.prototype.parseOptions = function (a) {
        var b = a.options.pieceLabel; return b ? (this.chartInstance = a, this.ctx = a.chart.ctx, this.options = a.config.options, this.render =
        b.render || b.mode, this.position = b.position || "default", this.arc = b.arc, this.format = b.format, this.precision = b.precision || 0, this.fontSize = b.fontSize || this.options.defaultFontSize, this.fontColor = b.fontColor || this.options.defaultFontColor, this.fontStyle = b.fontStyle || this.options.defaultFontStyle, this.fontFamily = b.fontFamily || this.options.defaultFontFamily, this.hasTooltip = a.tooltip._active && a.tooltip._active.length, this.showZero = b.showZero, this.overlap = b.overlap, this.images = b.images || [], this.showActualPercentages =
        b.showActualPercentages || !1, !0) : !1
    }, c.prototype.checkTextBound = function (a, b, p, l) { for (var h = this.labelBounds, f = 0; f < h.length; ++f) { for (var g = h[f], d = [[a, p], [a, l], [b, p], [b, l]], e = 0; e < d.length; ++e) { var c = d[e][0], k = d[e][1]; if (c >= g.left && c <= g.right && k >= g.top && k <= g.bottom) return !1 } d = [[g.left, g.top], [g.left, g.bottom], [g.right, g.top], [g.right, g.bottom]]; for (e = 0; e < d.length; ++e) if (c = d[e][0], k = d[e][1], c >= a && c <= b && k >= p && k <= l) return !1 } h.push({ left: a, right: b, top: p, bottom: l }); return !0 }, c.prototype.measureText = function (a) {
        return "object" ===
        typeof a ? { width: a.width, height: a.height } : this.ctx.measureText(a)
    }, c.prototype.fillText = function (a, b, p) { var c = this.ctx; "object" === typeof a ? c.drawImage(a, b.x - a.width / 2, b.y - a.height / 2, a.width, a.height) : (c.fillStyle = p, c.textBaseline = "top", c.textAlign = "center", c.fillText(a, b.x, b.y - this.fontSize / 2)) }, c.prototype.loadImage = function (a) { var b = new Image; b.src = a.src; b.width = a.width; b.height = a.height; return b }, c.prototype.drawArcText = function (a, b, c, l) {
        var h = this.ctx, f = c.x, g = c.y, d = c.startAngle; c = c.endAngle; h.save();
        h.translate(f, g); g = c - d; d += Math.PI / 2; c += Math.PI / 2; var e = d; f = this.measureText(a); d += (c - (f.width / b + d)) / 2; if (l || !(c - d > g)) if ("string" === typeof a) for (h.rotate(d), l = 0; l < a.length; l++) d = a.charAt(l), f = h.measureText(d), h.save(), h.translate(0, -1 * b), h.fillText(d, 0, 0), h.restore(), h.rotate(f.width / b); else h.rotate((e + c) / 2), h.translate(0, -1 * b), this.fillText(a, { x: 0, y: 0 }); h.restore()
    }, Chart.pluginService.register({
        beforeInit: function (a) { a.pieceLabel = new c }, beforeDatasetsUpdate: function (a) { a.pieceLabel.beforeDatasetsUpdate(a) },
        afterDatasetsDraw: function (a) { a.pieceLabel.afterDatasetsDraw(a) }
    }))
})();