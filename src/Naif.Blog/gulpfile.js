/// <binding AfterBuild='build' Clean='clean' />

var gulp = require("gulp");
var del = require("del");

var paths = {
    scripts: "./scripts/",
    cssSource: "./views/themes/**/*.css",
    cssTarget: "./wwwroot/themes/",
    imageSource: ["./views/themes/**/*.gif","./views/themes/**/*.jpg", "./views/themes/**/*.png"],
    imageTarget: "./wwwroot/themes/",
    libSource: "./node_modules/",
    libTarget: "./wwwroot/lib/",
    appTarget: "./wwwroot/app/"
}

var libs = [
];

gulp.task('build:bootstrap', function () {
    return gulp.src(paths.libSource + "bootstrap/dist/**/*.*").pipe(gulp.dest(paths.libTarget + "bootstrap/"));
});

gulp.task('build:bootstrap-tags', function () {
    return gulp.src(paths.libSource + "bootstrap-tagsinput/dist/**/*.*").pipe(gulp.dest(paths.libTarget + "bootstrap-tagsinput/"));
});

gulp.task('build:bootstrap-multiselect', function () {
    return gulp.src(paths.libSource + "bootstrap-multiselect/dist/**/*.*").pipe(gulp.dest(paths.libTarget + "bootstrap-multiselect/"));
});

gulp.task('build:ckeditor', function () {
    return gulp.src(paths.libSource + "ckeditor/**/*.*").pipe(gulp.dest(paths.libTarget + "ckeditor/"));
});

gulp.task('build:jquery', function () {
    return gulp.src(paths.libSource + "jquery/dist/**/*.*").pipe(gulp.dest(paths.libTarget + "jquery/"));
});

gulp.task('build:jqcloud', function () {
    return gulp.src(paths.libSource + "jqcloud-npm/dist/**/*.*").pipe(gulp.dest(paths.libTarget + "jqcloud/"));
});

gulp.task('build:css', function () {
    return gulp.src(paths.cssSource).pipe(gulp.dest(paths.cssTarget));
});

gulp.task('build:images', function () {
    return gulp.src(paths.imageSource).pipe(gulp.dest(paths.imageTarget));
});

gulp.task('build:libs', function () {
    return gulp.src(libs).pipe(gulp.dest(paths.libTarget));
});

gulp.task("build", ["build:bootstrap", "build:bootstrap-multiselect", "build:bootstrap-tags", "build:ckeditor", "build:jquery", "build:jqcloud", "build:css", "build:images", "build:libs"]);


gulp.task("clean", function() {
    del([paths.imageTarget, paths.cssTarget]);
})