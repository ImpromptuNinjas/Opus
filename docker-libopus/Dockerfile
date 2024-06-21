# git checkout
FROM alpine/git AS git
#RUN git clone --progress -b v1.5.2 --depth 1 https://gitlab.xiph.org/xiph/opus.git /src
RUN git clone --progress -b v1.5.2 --depth 1 https://github.com/xiph/opus.git /src
COPY opus_bindings_compat.* /src/src/
RUN cd /src && patch -p1 < /src/src/opus_bindings_compat.patch

# amd64-linux-musl
FROM amd64/alpine AS amd64-linux-musl
RUN apk add build-base libtool automake autoconf cmake
RUN --mount=type=bind,from=git,source=/src,target=/src \
	cd /src && \
	cmake -S /src -B /build \
	-DCMAKE_BUILD_TYPE=Release \
	-DCMAKE_FIND_ROOT_PATH_MODE_PROGRAM=NEVER \
	-DCMAKE_FIND_ROOT_PATH_MODE_LIBRARY=ONLY \
	-DCMAKE_FIND_ROOT_PATH_MODE_INCLUDE=ONLY \
	-DCMAKE_C_FLAGS="-g0 -O3 -flto -static-libgcc -fvisibility=hidden" \
	-DCMAKE_CXX_FLAGS="-g0 -O3 -flto -static-libgcc -fno-rtti -fvisibility=hidden -fvisibility-inlines-hidden" \
	-DBUILD_SHARED_LIBS=ON \
	&& cmake --build /build

RUN find /build/ -name 'lib*.so'

# amd64-linux-glibc
FROM amd64/debian:stable-slim AS amd64-linux-glibc
RUN apt-get update && apt-get install -y git autoconf automake libtool gcc make cmake
RUN --mount=type=bind,from=git,source=/src,target=/src \
	cd /src && \
	cmake -S /src -B /build \
	-DCMAKE_BUILD_TYPE=Release \
	-DCMAKE_FIND_ROOT_PATH_MODE_PROGRAM=NEVER \
	-DCMAKE_FIND_ROOT_PATH_MODE_LIBRARY=ONLY \
	-DCMAKE_FIND_ROOT_PATH_MODE_INCLUDE=ONLY \
	-DCMAKE_C_FLAGS="-g0 -O3 -flto -static-libgcc -fvisibility=hidden" \
	-DCMAKE_CXX_FLAGS="-g0 -O3 -flto -static-libgcc -fno-rtti -fvisibility=hidden -fvisibility-inlines-hidden" \
	-DBUILD_SHARED_LIBS=ON \
	&& cmake --build /build

# win64
FROM amd64/alpine AS win64
RUN apk add mingw-w64-gcc autoconf automake libtool make cmake patch
RUN --mount=type=bind,from=git,source=/src,target=/src \
	cd /src && \
	cmake -S /src -B /build \
	-DCMAKE_BUILD_TYPE=Release \
	-DCMAKE_SYSTEM_NAME=Windows \
	-DCMAKE_C_COMPILER=/usr/bin/x86_64-w64-mingw32-gcc \
	-DCMAKE_CXX_COMPILER=/usr/bin/x86_64-w64-mingw32-g++ \
	-DCMAKE_RC_COMPILER=/usr/bin/x86_64-w64-mingw32-windres \
	-DCMAKE_AR=/usr/bin/x86_64-w64-mingw32-gcc-ar \
	-DCMAKE_RANLIB=/usr/bin/x86_64-w64-mingw32-gcc-ranlib \
	-DCMAKE_FIND_ROOT_PATH=/usr/x86_64-w64-mingw32 \
	-DCMAKE_FIND_ROOT_PATH_MODE_PROGRAM=NEVER \
	-DCMAKE_FIND_ROOT_PATH_MODE_LIBRARY=ONLY \
	-DCMAKE_FIND_ROOT_PATH_MODE_INCLUDE=ONLY \
	-DENABLE_HARDENING=OFF \
	-DOPUS_STACK_PROTECTOR=OFF \
	-DCMAKE_C_FLAGS="-g0 -O3 -flto -static-libgcc -fvisibility=hidden -fstack-protector-strong -D_FORTIFY_SOURCE=2 -lssp_nonshared -static -lssp" \
	-DCMAKE_CXX_FLAGS="-g0 -O3 -flto -static-libgcc -fno-rtti -fvisibility=hidden -fvisibility-inlines-hidden -fstack-protector-strong -D_FORTIFY_SOURCE=2 -llibssp_nonshared -static -lssp" \
	-DBUILD_SHARED_LIBS=ON \
	&& cmake --build /build

# arm64-linux-musl
FROM arm64v8/alpine AS arm64-linux-musl
RUN apk add build-base libtool automake autoconf cmake
RUN --mount=type=bind,from=git,source=/src,target=/src \
	cd /src && \
	cmake -S /src -B /build \
	-DCMAKE_BUILD_TYPE=Release \
	-DCMAKE_FIND_ROOT_PATH_MODE_PROGRAM=NEVER \
	-DCMAKE_FIND_ROOT_PATH_MODE_LIBRARY=ONLY \
	-DCMAKE_FIND_ROOT_PATH_MODE_INCLUDE=ONLY \
	-DCMAKE_C_FLAGS="-g0 -O3 -flto -static-libgcc -fvisibility=hidden" \
	-DCMAKE_CXX_FLAGS="-g0 -O3 -flto -static-libgcc -fno-rtti -fvisibility=hidden -fvisibility-inlines-hidden" \
	-DBUILD_SHARED_LIBS=ON \
	&& cmake --build /build


# arm64-linux-glibc
FROM arm64v8/debian:stable-slim AS arm64-linux-glibc
RUN apt-get update && apt-get install -y  git autoconf automake libtool gcc make cmake
RUN --mount=type=bind,from=git,source=/src,target=/src \
	cd /src && \
	cmake -S /src -B /build \
	-DCMAKE_BUILD_TYPE=Release \
	-DCMAKE_FIND_ROOT_PATH_MODE_PROGRAM=NEVER \
	-DCMAKE_FIND_ROOT_PATH_MODE_LIBRARY=ONLY \
	-DCMAKE_FIND_ROOT_PATH_MODE_INCLUDE=ONLY \
	-DCMAKE_C_FLAGS="-g0 -O3 -flto -static-libgcc -fvisibility=hidden" \
	-DCMAKE_CXX_FLAGS="-g0 -O3 -flto -static-libgcc -fno-rtti -fvisibility=hidden -fvisibility-inlines-hidden" \
	-DBUILD_SHARED_LIBS=ON \
	&& cmake --build /build

FROM scratch as finale

COPY --from=amd64-linux-musl /build/libopus.so /app/linux-musl-x64/native/
COPY --from=amd64-linux-glibc /build/libopus.so /app/linux-x64/native/

COPY --from=win64 /build/libopus.dll /app/win-x64/native/

COPY --from=arm64-linux-musl /build/libopus.so /app/linux-musl-arm64/native/
COPY --from=arm64-linux-glibc /build/libopus.so /app/linux-arm64/native/

# now we just extract the app dir as an artifact
ENTRYPOINT ["/bin/sh"]