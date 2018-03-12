const webpack = require('webpack');
const path = require('path');
const HtmlPlugin = require('html-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');

module.exports = options => {
    const scriptBundleFileName = options.release ? 'scripts-[hash].js' : 'scripts.js';
    const styleBundleFileName = options.release ? 'styles-[hash].css' : 'styles.css';

    const plugins = [];

    plugins.push(new webpack.DefinePlugin({
        "process.env.NODE_ENV": JSON.stringify(options.release ? 'production' : 'staging')
    }));

    return {
        devtool: 'cheap-module-source-map',
        entry: ['babel-polyfill', './Client/entry.jsx'],

        module: {
            rules: [
                {
                    test: /\.jsx?$/,
                    exclude: /node_modules/,
                    loader: 'babel-loader',
                    query: {
                        plugins: ['transform-decorators-legacy', 'transform-export-extensions'],
                        presets: ['es2015', 'react', 'stage-2']
                    }
                },
                {
                  test: /\.css$/,
                  use: ExtractTextPlugin.extract({
                    fallback: "style-loader",
                    use: "css-loader?modules"
                  })
                },
                {
                    test: /\.(eot|svg|ttf|woff|woff2|png|jpg|svg)(\?.*$|$)/,
                    loader: 'file-loader'
                }
            ]
        },

        resolve: {
            modules: [path.resolve(__dirname, 'Client'), 'node_modules'],
            extensions: ['*', '.js', '.jsx', '.css']
        },

        output: {
            path: path.join(__dirname, '/client-build/'),
            publicPath: '/client-build/',
            filename: scriptBundleFileName
        },

        plugins: [
            ...plugins,
            new HtmlPlugin({
                title: '',
                filename: '../Views/Home/Index.cshtml', // relative to /client-build
                template: 'Views/Home/Index.Template.cshtml',
                inject: 'body'
            }),
            new ExtractTextPlugin(styleBundleFileName, {
                allChunks: true
            })
        ]
    }
}
